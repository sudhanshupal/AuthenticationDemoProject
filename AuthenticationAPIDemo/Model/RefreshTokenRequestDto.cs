namespace AuthenticationAPIDemo.Model
{
    public class RefreshTokenRequestDto
    {
        public Guid UserId { get; set; } 

        public string AccessTokne { get; set; } = string.Empty;
        public string RefreshTokne { get; set; } = string.Empty;

    }
}
