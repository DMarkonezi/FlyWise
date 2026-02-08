namespace Api.Models.Auth
{
    public class AuthResponse
    {
        public string? Token { get; set; }
        public UserDto? User { get; set; }
        public int ExpiresIn { get; set; }
    }
}