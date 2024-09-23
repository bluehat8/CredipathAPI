namespace CredipathAPI.Helpers
{
    public class PermissionDTO
    {
        public int Id { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        //public ICollection<UserPermissionDTO> UserPermissions { get; set; }
    }
}
