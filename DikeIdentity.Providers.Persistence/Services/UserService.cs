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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _classicHasher;
        private readonly IPasswordHasher _argonHasher;
        private readonly ILogger<UserService> _logger;

        public UserService(
            ILogger<UserService> logger,
            IUserRepository userRepository,
            IJwtProvider jwtProvider,
            [FromKeyedServices("Classic")] IPasswordHasher classicHasher,
            [FromKeyedServices("Hardened")] IPasswordHasher argonHasher)
        {
            _userRepository = userRepository;
            _classicHasher = classicHasher;
            _argonHasher = argonHasher;
            _logger = logger;
        }

        public async Task<Guid> RegisterStandardAsync(RegisterRequest request)
        {
            return await CreateUserInternal(request, _classicHasher, AuthProvider.local);
        }

        public async Task<Guid> RegisterWithArgon2Async(RegisterRequest request)
        {
            return await CreateUserInternal(request, _argonHasher, AuthProvider.local);
        }

        private async Task<Guid> CreateUserInternal(RegisterRequest request, IPasswordHasher hasher, AuthProvider provider)
        {
            // 1. Validar si el correo ya existe
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new IdentityException("El correo ya está registrado.", "REG_001");
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
            bool execute = await _userRepository.AddAsync(user);
            
            if(!execute)
                throw new IdentityException("Error al registrar el usuario. Intente nuevamente.", "REG_002");

            return user.Id;
        }
    }
}
