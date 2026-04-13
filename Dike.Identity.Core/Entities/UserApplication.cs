using Dike.Identity.Core.Enums;

namespace Dike.Identity.Core.Entities
{
    public class UserApplication
    {
        public Guid UserId { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid RoleId { get; set; }
        public StateStatus Status { get; set; }
        public DateTime AssignedAt { get; set; }
        public Guid? AssignedBy { get; set; }

        public virtual User? User { get; set; }
        public virtual Application? Application { get; set; }
        public virtual Role? Role { get; set; }

    }
}
