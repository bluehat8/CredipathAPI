using System.ComponentModel.DataAnnotations;

namespace CredipathAPI.DTOs
{
    public class RouteDTO
    {
        [Required(ErrorMessage = "El nombre de la ruta es requerido")]
        public string route_name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string description { get; set; } = string.Empty;

    }
}
