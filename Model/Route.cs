using CredipathAPI.Base;
using static CredipathAPI.Constants;

namespace CredipathAPI.Model
{
    public class Route : BaseEntity
    {
        public string? route_name { get; set; } 
        public string? description { get; set; }
        public required ICollection<Client> Clients { get; set; }
    }
}
