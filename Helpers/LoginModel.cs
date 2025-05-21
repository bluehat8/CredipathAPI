namespace CredipathAPI.Helpers
{
    public class LoginModel
    {
        public required string usernameOrEmail { get; set; }
        public required string password { get; set; }
    }
}
