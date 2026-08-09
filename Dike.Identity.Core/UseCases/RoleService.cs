using Dike.Identity.Core.DTOs.Role;
using Dike.Identity.Core.Entities;
using Dike.Identity.Core.Interfaces.Repositories;
using Dike.Identity.Core.Interfaces.Services;

namespace Dike.Identity.Core.UseCases
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task RegisterAsync(RoleRegisterRequest request)
        {
            await _roleRepository.RegisterAsync(new Role
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                Name = request.Name,
                NormalizedName = request.Name.ToUpperInvariant(),
                Description = request.Description,
                IsDefault = request.IsDefault,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = null
            });
        }
    }
}
