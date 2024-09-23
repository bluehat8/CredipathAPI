using CredipathAPI.Base;

namespace CredipathAPI.Model
{
    public class UserRoute:BaseEntity
    {
        public int UserId { get; set; }
        public int RouteId { get; set; }
        public required User User { get; set; }
        public required Route Route { get; set; }
    }
}
