using System.ComponentModel.DataAnnotations;

namespace CredipathAPI.DTOs
{
    public class UpdateRouteDTO
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre de la ruta es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El distrito es requerido")]
        [MaxLength(100, ErrorMessage = "El distrito no puede tener más de 100 caracteres")]
        public string District { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ubicación es requerida")]
        [MaxLength(200, ErrorMessage = "La ubicación no puede tener más de 200 caracteres")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es requerido")]
        [RegularExpression("active|inactive", ErrorMessage = "El estado debe ser 'active' o 'inactive'")]
        public string Status { get; set; } = "active";
    }
}
