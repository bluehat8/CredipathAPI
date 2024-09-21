using CredipathAPI.Base;
using static CredipathAPI.Constants;

namespace CredipathAPI.Model
{
    public class Client : BaseEntity
    {
        public string? name { get; set; }    
        public string? code { get; set; }    
        public string? email { get; set; }   
        public string? phone { get; set; }
        public string? notes { get; set; }
        public string? address { get; set; }
        public int? creator { get; set; }
        public int? RouteId { get; set; }
        public virtual Route? Route { get; set; }

    }
}
