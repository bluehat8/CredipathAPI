namespace CredipathAPI.Helpers
{
    public class UserPermissionDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PermissionId { get; set; }
        public string PermissionModule { get; set; } // Ej: Prestamos, Pagos
        public string PermissionAction { get; set; } // Ej: Agregar, Editar, Eliminar, Abonar

    }
}
