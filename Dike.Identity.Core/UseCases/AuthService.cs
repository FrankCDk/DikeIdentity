using Dike.Identity.Core.Common;
using Dike.Identity.Core.DTOs.Auth;
using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Enums;
using Dike.Identity.Core.Interfaces.Repositories;
using Dike.Identity.Core.Interfaces.Security;
using Dike.Identity.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dike.Identity.Core.UseCases
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordHasher _classicHasher;
        private readonly IPasswordHasher _argonHasher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            ILogger<AuthService> logger,
            IUserRepository userRepository,
            IApplicationRepository applicationRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtProvider jwtProvider,
            [FromKeyedServices("Classic")] IPasswordHasher classicHasher,
            [FromKeyedServices("Hardened")] IPasswordHasher argonHasher)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtProvider = jwtProvider;
            _classicHasher = classicHasher;
            _argonHasher = argonHasher;
            _logger = logger;
            _applicationRepository = applicationRepository;
        }

        #region Login Methods

        public async Task<Response<AuthResponse>>   LoginStandardAsync(LoginRequest request)
        {
            return await ExecuteLogin(request, _classicHasher, "LOGIN_CLASSIC");
        }

        public async Task<Response<AuthResponse>> LoginWithArgon2Async(LoginRequest request)
        {
            return await ExecuteLogin(request, _argonHasher, "LOGIN_ARGON2ID");
        }

        private async Task<Response<AuthResponse>> ExecuteLogin(LoginRequest request, IPasswordHasher hasher, string auditAction)
        {
            // 1. Buscar usuario
            _logger.LogInformation("Buscando correo en base de datos: {Email}", request.Email);
            var user = await _userRepository.GetByEmailAsync(request.Email);

            // MITIGACIÓN DE TIMING ATTACK:
            if (user == null)
            {
                string hashFalsoEstructuralmenteValido = hasher.HashPassword("UnaContrasenaCualquieraParaMitigarTimingAttacks");
                hasher.VerifyPassword(request.Password, hashFalsoEstructuralmenteValido);
                return Response<AuthResponse>.Failure(AuthErrors.InvalidCredentials);
            }

            // 2. Verificar si la cuenta está bloqueada por intentos fallidos
            if (user.IsLocked && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                _logger.LogWarning("Intento de login para usuario bloqueado: {Email}", request.Email);
                return Response<AuthResponse>.Failure(AuthErrors.AccountLocked);
            }

            // Verificar password con el hasher seleccionado
            if (!hasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogInformation("Intento de login fallido para usuario: {Email}", request.Email);

                // Aumentamos la cantidad de intentos fallidos en la tabla de usuarios
                bool accountLocked = await _userRepository.FailedLoginAttemptAsync(user.Id);

                if (accountLocked)
                {
                    _logger.LogWarning("La cuenta del usuario {Email} ha sido bloqueada por múltiples intentos fallidos.", request.Email);
                    return Response<AuthResponse>.Failure(AuthErrors.AccountLocked);
                }

                return Response<AuthResponse>.Failure(AuthErrors.InvalidCredentials);
            }

            // Si el login es exitoso, reseteamos los intentos fallidos (si es que había)
            if (user.FailedLoginAttempts > 0 || user.IsLocked)
            {
                user.FailedLoginAttempts = 0;
                user.IsLocked = false;
                user.LockoutEnd = null;
                await _userRepository.UpdateAsync(user);
            }

            // Obtenemos el acceso del usuario a la aplicación específica
            var userAppAccess = user.UserApplications.FirstOrDefault(ua => ua.ApplicationId == request.ClientId && ua.Status == StateStatus.active);

            if (userAppAccess == null)
            {
                _logger.LogWarning("Intento de login para usuario sin acceso a la aplicación: {Email}", request.Email);
                return Response<AuthResponse>.Failure(new Error("AUTH_008", "No tienes permisos para acceder a esta aplicación."));
            }

            // Obtenemos el secret de la aplicacion
            var app = await _applicationRepository.GetByIdAsync(request.ClientId);

            if(app == null)
            {

                return Response<AuthResponse>.Failure(new Error("AUTH_009", "Aplicación no encontrada."));
            }

            // 4. Generar Tokens
            var authResponse = _jwtProvider.GenerateTokens(user, app.SecretHash, app.Id.ToString());

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ApplicationId = app.Id,
                Token = authResponse.RefreshToken,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7), // Dura una semana
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            _logger.LogInformation("Login exitoso para {Email}. Refresh Token persistido.", user.Email);

            return Response<AuthResponse>.Ok(authResponse, "Autenticación exitosa.");
        }

        #endregion

        #region Refresh Token

        public async Task<Response<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            // 1. Obtener el refresh token de la base de datos
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (storedToken == null)
                return Response<AuthResponse>.Failure(AuthErrors.TokenNotFound);

            if (storedToken.ExpiresAt < DateTime.UtcNow)
                return Response<AuthResponse>.Failure(AuthErrors.TokenExpired);

            if (storedToken.RevokedAt != null)
                return Response<AuthResponse>.Failure(AuthErrors.TokenRevoked);

            // 2. Obtener el usuario asociado
            var user = await _userRepository.GetByIdAsync(storedToken.UserId);
            if (user == null)
                return Response<AuthResponse>.Failure(AuthErrors.AssociatedUserNotFound);

            // Verificar si el usuario aún tiene acceso a la aplicación
            var userAppAccess = user.UserApplications.FirstOrDefault(ua => ua.ApplicationId == storedToken.ApplicationId && ua.Status == StateStatus.active);

            if (userAppAccess == null)
            {
                _logger.LogWarning("Intento de Refresh Token para un usuario que perdió acceso a la aplicación. Usuario: {UserId}", user.Id);
                return Response<AuthResponse>.Failure(new Error("AUTH_010", "Tu acceso a esta aplicación ha sido revocado."));
            }

            // Verificar si la aplicación aún está activa
            var app = await _applicationRepository.GetByIdAsync(storedToken.ApplicationId);
            if (app == null || app.Status != StateStatus.active)
            {
                return Response<AuthResponse>.Failure(new Error("AUTH_009", "La aplicación ya no está disponible o está inactiva."));
            }

            // 3. Generar nuevos tokens
            var authResponse = _jwtProvider.GenerateTokens(user, app.SecretHash, app.Id.ToString());

            // 4. Revocar el refresh token antiguo
            storedToken.RevokedAt = DateTimeOffset.UtcNow;
            await _refreshTokenRepository.UpdateAsync(storedToken);

            // 5. Persistir el nuevo refresh token
            var newRefreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ApplicationId = app.Id,
                Token = authResponse.RefreshToken,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);

            return Response<AuthResponse>.Ok(authResponse, "Tokens renovados exitosamente.");
        }

        #endregion



    }
}
