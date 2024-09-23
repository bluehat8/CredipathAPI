using CredipathAPI.Base;

namespace CredipathAPI.Model
{
    public class UserRoute:BaseEntity
    {
        public int UserId { get; set; }
        public int RouteId { get; set; }
        public User User { get; set; }
        public Route Route { get; set; }
    }
}
