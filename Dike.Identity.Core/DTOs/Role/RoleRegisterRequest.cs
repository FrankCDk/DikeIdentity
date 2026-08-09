using Dike.Identity.Core.Enums;

namespace Dike.Identity.Core.DTOs.Role
{
    public record RoleRegisterRequest(
        string Code,
        string Name,
        string Description,
        bool IsDefault,
        StateStatus Status
    );
}
