namespace CredipathAPI.Model
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public required string Module { get; set; } // Ej: Prestamos, Pagos
        public required string Action { get; set; } // Ej: Agregar, Editar, Eliminar, Abonar

        public required ICollection<UserPermission> UserPermissions { get; set; }
    }

}
