using CredipathAPI.Base;

namespace CredipathAPI.Model
{
    public class UserPermission:BaseEntity
    {
        public int UserId { get; set; }
        public int PermissionId { get; set; }

        public User User { get; set; }
        public Permission Permission { get; set; }
    }
}
