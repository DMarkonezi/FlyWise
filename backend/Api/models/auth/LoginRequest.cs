using System.Text.Json.Serialization;

namespace Api.Models.Auth
{
    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}