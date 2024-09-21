namespace CredipathAPI.Model
{
    public class UserPermission
    {
        public int UserPermissionId { get; set; }
        public int UserId { get; set; }
        public int PermissionId { get; set; }

        public User User { get; set; }
        public Permission Permission { get; set; }
    }
}
