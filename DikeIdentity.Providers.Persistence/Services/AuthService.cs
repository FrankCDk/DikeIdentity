using Dike.Identity.Core.Common;
using Dike.Identity.Core.DTOs.Auth;
using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Interfaces.Repositories;
using Dike.Identity.Core.Interfaces.Security;
using Dike.Identity.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dike.Identity.Providers.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordHasher _classicHasher;
        private readonly IPasswordHasher _argonHasher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            ILogger<AuthService> logger,
            IUserRepository userRepository,
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
        }

        #region Login Methods

        public async Task<AuthResponse> LoginStandardAsync(LoginRequest request)
        {
            return await ExecuteLogin(request, _classicHasher, "LOGIN_CLASSIC");
        }

        public async Task<AuthResponse> LoginWithArgon2Async(LoginRequest request)
        {
            return await ExecuteLogin(request, _argonHasher, "LOGIN_ARGON2ID");
        }

        private async Task<AuthResponse> ExecuteLogin(LoginRequest request, IPasswordHasher hasher, string auditAction)
        {
            // 1. Buscar usuario
            _logger.LogInformation("Buscando correo en base de datos: {Email}", request.Email);
            var user = await _userRepository.GetByEmailAsync(request.Email);

            // MITIGACIÓN DE TIMING ATTACK:
            // Si el usuario no existe, creamos un "usuario fantasma" con un hash falso 
            // para que el Hasher trabaje de todos modos y el tiempo de respuesta sea idéntico.
            if (user == null)
            {
                hasher.VerifyPassword(request.Password, "HashFalsoParaEngañarAlAtacante_Argon2id_PBKDF2");
                throw new IdentityException("AUTH_001", "Credenciales inválidas");
            }

            // 2. Verificar si la cuenta está bloqueada por intentos fallidos
            if (user.IsLocked && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                _logger.LogWarning("Intento de login para usuario bloqueado: {Email}", request.Email);
                throw new IdentityException("AUTH_002", "Cuenta bloqueada por múltiples intentos fallidos. Intente nuevamente más tarde.");
            }

            // 3. Verificar password con el hasher seleccionado
            if (!hasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                // Logueamos el intento fallido para auditoría
                _logger.LogInformation("Intento de login fallido para usuario: {Email}", request.Email);

                // Aumentamos la cantidad de intentos fallidos en la tabla de usuarios
                bool accountLocked = await _userRepository.FailedLoginAttemptAsync(user.Id);

                if (accountLocked)
                {
                    _logger.LogWarning("La cuenta del usuario {Email} ha sido bloqueada por múltiples intentos fallidos.", request.Email);
                    throw new IdentityException("AUTH_002", "Cuenta bloqueada por múltiples intentos fallidos. Intente nuevamente más tarde.");
                }

                throw new IdentityException("AUTH_001", "Credenciales inválidas");

            }

            // 3. Si el login es exitoso, reseteamos los intentos fallidos (si es que había)
            if (user.FailedLoginAttempts > 0 || user.IsLocked)
            {
                user.FailedLoginAttempts = 0;
                user.IsLocked = false;
                user.LockoutEnd = null;
                await _userRepository.UpdateAsync(user);
            }

            // 4. Generar Tokens
            var authResponse = _jwtProvider.GenerateTokens(user);

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = authResponse.RefreshToken,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7), // Dura una semana
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            _logger.LogInformation("Login exitoso para {Email}. Refresh Token persistido.", user.Email);

            return authResponse;
        }

        #endregion

        #region Refresh Token

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            // 1. Obtener el refresh token de la base de datos
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (storedToken == null)
                throw new UnauthorizedAccessException("El Refresh Token no existe.");

            if (storedToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("El Refresh Token ha expirado.");

            if (storedToken.RevokedAt != null)
                throw new UnauthorizedAccessException("El Refresh Token ha sido revocado.");

            // 2. Obtener el usuario asociado
            var user = await _userRepository.GetByIdAsync(storedToken.UserId);
            if (user == null)
                throw new UnauthorizedAccessException("El usuario asociado al Refresh Token no existe.");

            // 3. Generar nuevos tokens
            var authResponse = _jwtProvider.GenerateTokens(user);

            // 4. Revocar el refresh token antiguo
            storedToken.RevokedAt = DateTimeOffset.UtcNow;
            await _refreshTokenRepository.UpdateAsync(storedToken);

            // 5. Persistir el nuevo refresh token
            var newRefreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = authResponse.RefreshToken,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);

            return authResponse;
        }

        #endregion



    }
}
