using CredipathAPI.Model;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CredipathAPI.DTOs
{
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
        
        // La verdad esto no lo vamos a eliminar luego
        public string ConfirmPassword { get; set; }
        
        public string Role { get; set; } = "collaborator";
        
        // Ahora se puede enviar como 'permissions' desde el cliente
        [JsonPropertyName("permissions")]
        public List<int>? PermissionIds { get; set; }
        
        public NestedPermissionsDTO Permissions { get; set; }
    }
    
    // DTO para la estructura anidada de permisos que viene del cliente
    public class NestedPermissionsDTO
    {
        public LoanPermissionsDTO Loan { get; set; }
        public PaymentPermissionsDTO Payment { get; set; }
        public ModulesPermissionsDTO Modules { get; set; }
    }
    
    public class LoanPermissionsDTO
    {
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
    }
    
    public class PaymentPermissionsDTO
    {
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
    }
    
    public class ModulesPermissionsDTO
    {
        public bool Collaborators { get; set; }
        public bool OverduePayments { get; set; }
        public bool UpcomingPayments { get; set; }
        public bool LoanPayment { get; set; }
        public bool Report { get; set; }
    }
    
    // DTO para actualización de colaborador
    public class CollaboratorUpdateDTO
    {
        public string Identifier { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        
        // IDs de permisos a asignar (reemplazarán los permisos existentes)
        public List<int>? Permissions { get; set; }
    }

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
        
        // Permisos asignados
        public List<PermissionDTO> Permissions { get; set; } = new List<PermissionDTO>();
    }

    public class PermissionDTO
    {
        public int Id { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
