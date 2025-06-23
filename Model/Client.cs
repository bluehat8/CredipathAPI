using CredipathAPI.Base;
using System.ComponentModel.DataAnnotations;
using static CredipathAPI.Constants;

namespace CredipathAPI.Model
{
    public class Client : BaseEntity
    {
        [Required]
        [MaxLength(20)]
        public string Identification { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public string? Code { get; set; }
        
        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }
        
        [MaxLength(20)]
        public string? Phone { get; set; }
        
        [MaxLength(20)]
        public string? LandlinePhone { get; set; }
        
        public string? Notes { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string HomeAddress { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string? BusinessAddress { get; set; }
        
        [MaxLength(20)]
        public string? Gender { get; set; }
        
        [MaxLength(100)]
        public string? Municipality { get; set; }
        
        [MaxLength(100)]
        public string? City { get; set; }
        
        [MaxLength(100)]
        public string? Neighborhood { get; set; }
        
        public int? CreatorId { get; set; }
        public int? RouteId { get; set; }
        public virtual Route? Route { get; set; }
    }
}
