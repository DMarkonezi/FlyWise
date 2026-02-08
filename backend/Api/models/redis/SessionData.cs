namespace Api.Models.Redis
{
    public class SessionData
    {
        public string? Token { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? LoginTime { get; set; }
        public string? LastActivity { get; set; }
    }
}