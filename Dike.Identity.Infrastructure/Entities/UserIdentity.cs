using Microsoft.AspNetCore.Identity;

namespace Dike.Identity.Infrastructure.Entities
{
    public class UserIdentity : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; } = null;
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; } = null;
    }
}
