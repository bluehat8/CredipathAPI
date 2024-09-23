namespace CredipathAPI.Helpers
{
    public class UserDTO
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string? Note { get; set; }
        public string? Address { get; set; }
        public string? Code { get; set; }
        public List<int> RouteIds { get; set; } // Rutas a las que se asignará el colaborador
        public List<int> PermissionIds { get; set; } // Permisos asignados al colaborador
    }

}
