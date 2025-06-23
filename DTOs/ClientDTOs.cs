using System.ComponentModel.DataAnnotations;

namespace CredipathAPI.DTOs
{
    public class CreateClientDTO
    {
        [Required(ErrorMessage = "La identificación es requerida")]
        [MaxLength(20, ErrorMessage = "La identificación no puede tener más de 20 caracteres")]
        public string Identification { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "El código no puede tener más de 100 caracteres")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "El ID de la ruta es requerido")]
        public int RouteId { get; set; }

        [Required(ErrorMessage = "La dirección domiciliar es requerida")]
        [MaxLength(200, ErrorMessage = "La dirección no puede tener más de 200 caracteres")]
        public string HomeAddress { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "La dirección de negocio no puede tener más de 200 caracteres")]
        public string? BusinessAddress { get; set; }

        [MaxLength(20, ErrorMessage = "El género no puede tener más de 20 caracteres")]
        public string? Gender { get; set; }

        [MaxLength(100, ErrorMessage = "El municipio no puede tener más de 100 caracteres")]
        public string? Municipality { get; set; }

        [MaxLength(100, ErrorMessage = "La ciudad no puede tener más de 100 caracteres")]
        public string? City { get; set; }

        [MaxLength(100, ErrorMessage = "El barrio no puede tener más de 100 caracteres")]
        public string? Neighborhood { get; set; }

        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [MaxLength(100, ErrorMessage = "El correo electrónico no puede tener más de 100 caracteres")]
        public string? Email { get; set; }

        [MaxLength(20, ErrorMessage = "El teléfono celular no puede tener más de 20 caracteres")]
        public string? Phone { get; set; }

        [MaxLength(20, ErrorMessage = "El teléfono fijo no puede tener más de 20 caracteres")]
        public string? LandlinePhone { get; set; }

        [MaxLength(500, ErrorMessage = "Las notas no pueden tener más de 500 caracteres")]
        public string? Notes { get; set; }
    }

    public class UpdateClientDTO : CreateClientDTO
    {
        // Puede incluir campos específicos de actualización si es necesario
    }

    public class ClientResponseDTO
    {
        public int Id { get; set; }
        public string Identification { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public int RouteId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public string HomeAddress { get; set; } = string.Empty;
        public string? BusinessAddress { get; set; }
        public string? Gender { get; set; }
        public string? Municipality { get; set; }
        public string? City { get; set; }
        public string? Neighborhood { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? LandlinePhone { get; set; }
        public string? Notes { get; set; }
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
