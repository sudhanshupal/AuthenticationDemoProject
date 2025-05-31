using AuthenticationAPIDemo.Entities;
using AuthenticationAPIDemo.Model;

namespace AuthenticationAPIDemo.Services
{
    public interface IAuthService
    {
        Task<TokenResponseDto> LoginAsync(UserDto request);
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}