namespace CredipathAPI.DTOs
{
    public class UserRouteDTO
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public int RouteId { get; set; }
        public string? RouteName { get; set; }
        public string? RouteDescription { get; set; }
    }

}
