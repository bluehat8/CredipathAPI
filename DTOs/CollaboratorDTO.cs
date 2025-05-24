using CredipathAPI.Model;
using System.ComponentModel.DataAnnotations;

namespace CredipathAPI.DTOs
{
    // DTO para creación de colaborador (crea también el usuario asociado)
    public class CollaboratorCreateDTO
    {
        // Datos del colaborador
        [Required(ErrorMessage = "El identificador es obligatorio")]
        public string Identifier { get; set; }
        
        public string Phone { get; set; }
        public string Mobile { get; set; }
        
        // Datos para el usuario asociado
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        public string Email { get; set; }
        
        public string Address { get; set; }
        
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }
        
        // No usamos esto pero lo dejamos por compatibilidad con clientes existentes
        public string ConfirmPassword { get; set; }
        
        // El rol siempre será "collaborator"
        public string Role { get; set; } = "collaborator";
        
        // Permisos
        public PermissionsDTO Permissions { get; set; }
    }
    
    // DTO para los permisos
    public class PermissionsDTO
    {
        public LoanPermissions Loan { get; set; }
        public PaymentPermissions Payment { get; set; }
        public ModulePermissions Modules { get; set; }
    }
    
    // DTO para actualización de colaborador
    public class CollaboratorUpdateDTO
    {
        public string Identifier { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        
        // Permisos
        public LoanPermissions Loan { get; set; }
        public PaymentPermissions Payment { get; set; }
        public ModulePermissions Modules { get; set; }
    }

    // Para respuesta
    public class CollaboratorResponseDTO
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        
        // Datos del usuario asociado
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        
        // Información de creación
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Permisos
        public LoanPermissions Loan { get; set; }
        public PaymentPermissions Payment { get; set; }
        public ModulePermissions Modules { get; set; }
    }

    // Clase para respuestas estándar de la API
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
