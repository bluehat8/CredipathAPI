using CredipathAPI.Base;
using static CredipathAPI.Constants;
    
namespace CredipathAPI.Model
{
    public class User:BaseEntity
    {
        public string? name { get; set; }
        public string? username { get; set; }
        public string? email { get; set; }
        public DateTime? emailVerifiedAt { get; set; }
        public string? password { get; set; }
        public string? rememberToken { get; set; }
        public int? currentTeamId { get; set; }
        public UserType UserType { get; set; }
        public ICollection<UserPermission> UserPermissions { get; set; }
        public ICollection<UserRoute> UserRoutes { get; set; }


    }
}
