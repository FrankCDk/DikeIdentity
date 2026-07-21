using Dike.Identity.Core.Common;
using Dike.Identity.Core.DTOs.Auth;
using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Enums;
using Dike.Identity.Core.Interfaces.Repositories;
using Dike.Identity.Core.Interfaces.Security;
using Dike.Identity.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dike.Identity.Core.UseCases
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _classicHasher;
        private readonly IPasswordHasher _argonHasher;

        public UserService(
            IUserRepository userRepository,
            IJwtProvider jwtProvider,
            [FromKeyedServices("Classic")] IPasswordHasher classicHasher,
            [FromKeyedServices("Hardened")] IPasswordHasher argonHasher)
        {
            _userRepository = userRepository;
            _classicHasher = classicHasher;
            _argonHasher = argonHasher;
        }

        public async Task<Response<Guid>> RegisterStandardAsync(RegisterRequest request)
        {
            return await CreateUserInternal(request, _classicHasher, AuthProvider.local);
        }

        public async Task<Response<Guid>> RegisterWithArgon2Async(RegisterRequest request)
        {
            return await CreateUserInternal(request, _argonHasher, AuthProvider.local);
        }

        private async Task<Response<Guid>> CreateUserInternal(RegisterRequest request, IPasswordHasher hasher, AuthProvider provider)
        {
            // 1. Validar si el correo ya existe
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                return Response<Guid>.Failure(UserErrors.EmailAlreadyExists);
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

            if (!execute)
            {
                return Response<Guid>.Failure(UserErrors.ErrorRegister);
            }

            return Response<Guid>.Ok(user.Id, "Usuario registrado exitosamente.");
        }
    }
}
