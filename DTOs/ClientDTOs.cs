using System.ComponentModel.DataAnnotations;

namespace CredipathAPI.DTOs
{
    public class CreateClientDTO
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [MaxLength(100, ErrorMessage = "El apellido no puede tener más de 100 caracteres")]
        public string Lastname { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ID de la ruta es requerido")]
        public int RouteId { get; set; }

        [Required(ErrorMessage = "La dirección es requerida")]
        [MaxLength(200, ErrorMessage = "La dirección no puede tener más de 200 caracteres")]
        public string Direction { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de celular es requerido")]
        [RegularExpression("^[0-9]{8}$", ErrorMessage = "El número de celular debe tener 8 dígitos")]
        public string Cellphone { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [MaxLength(100, ErrorMessage = "El correo electrónico no puede tener más de 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La nota no puede tener más de 500 caracteres")]
        public string? Note { get; set; }

        [RegularExpression("^[0-9]{8}$", ErrorMessage = "El número de teléfono fijo debe tener 8 dígitos")]
        public string? LandlinePhone { get; set; }
    }

    public class UpdateClientDTO : CreateClientDTO
    {
        // Puede incluir campos específicos de actualización si es necesario
    }

    public class ClientResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public int RouteId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public string Cellphone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string? LandlinePhone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ClientQueryParams
    {
        [Range(1, int.MaxValue, ErrorMessage = "El número de página debe ser mayor a 0")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100")]
        public int PageSize { get; set; } = 10;

        public string? SearchTerm { get; set; }
        public int? RouteId { get; set; }
    }
}
