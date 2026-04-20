using Dike.Identity.Core.Common;
using Dike.Identity.Core.DTOs.Auth;
using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Enums;
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
        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordHasher _classicHasher;
        private readonly IPasswordHasher _argonHasher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            ILogger<AuthService> logger,
            IUserRepository userRepository,
            IJwtProvider jwtProvider,
            [FromKeyedServices("Classic")] IPasswordHasher classicHasher,
            [FromKeyedServices("Hardened")] IPasswordHasher argonHasher)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
            _classicHasher = classicHasher;
            _argonHasher = argonHasher;
            _logger = logger;
        }


        #region Registro de usuarios
        public async Task<bool> RegisterStandardAsync(RegisterRequest request)
        {
            return await CreateUserInternal(request, _classicHasher, AuthProvider.local);
        }

        public async Task<bool> RegisterWithArgon2Async(RegisterRequest request)
        {
            return await CreateUserInternal(request, _argonHasher, AuthProvider.local);
        }

        private async Task<bool> CreateUserInternal(RegisterRequest request, IPasswordHasher hasher, AuthProvider provider)
        {
            // 1. Validar si el correo ya existe          
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new Exception("El correo ya está registrado."); // Luego usaremos IdentityException
            }

            // 2. Mapear DTO a Entidad
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email.ToLower().Trim(),
                NormalizedEmail = request.Email.ToUpper().Trim(),
                Name = request.Name,
                LastName = request.LastName,
                AuthProvider = provider,
                State = StateStatus.active,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = hasher.HashPassword(request.Password)
            };

            // 4. Persistir en Base de Datos
            await _userRepository.AddAsync(user);
            return true;
        }

        #endregion

        #region Login de usuarios
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
            if (user == null) throw new IdentityException("Credenciales inválidas", "AUTH_001");

            // 2. Verificar password con el hasher seleccionado
            if (!hasher.VerifyPassword(request.Password, user.PasswordHash))
                throw new IdentityException("Credenciales inválidas", "AUTH_001");

            // 3. Generar Tokens
            return _jwtProvider.GenerateTokens(user);
        }
        #endregion

    }
}
