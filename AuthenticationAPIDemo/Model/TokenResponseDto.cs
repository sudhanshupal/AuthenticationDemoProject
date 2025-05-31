namespace AuthenticationAPIDemo.Model
{
    public class TokenResponseDto
    {
        public string RefreshToken { get; set; } = string.Empty;

        public string AccessTokne { get; set; } = string.Empty;
    }
}
