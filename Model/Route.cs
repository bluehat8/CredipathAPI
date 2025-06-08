using CredipathAPI.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CredipathAPI.Model
{
    public class Route : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string route_name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? description { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string District { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;
        
        // Propiedades de navegación (no se incluyen en el DTO)
        public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
        public virtual ICollection<UserRoute> UserRoutes { get; set; } = new List<UserRoute>();
    }
}
