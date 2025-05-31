using AuthenticationAPIDemo.Data;
using AuthenticationAPIDemo.Entities;
using AuthenticationAPIDemo.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticationAPIDemo.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration configuration;
        private readonly MyDBContext context;
        public AuthService(IConfiguration congiguration, MyDBContext context)
        {
            this.configuration = congiguration;
            this.context = context;
        }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            if (await context.Users.AnyAsync(u => u.Username == request.Username))
                return null;
            var user = new User();
            user.Username = request.Username;
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return user;
        }
        public async Task<TokenResponseDto> LoginAsync(UserDto request)
        {
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
                return null;
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
                return null;
            var token = new TokenResponseDto
            {
                AccessTokne = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshToken(user)
            };
            return token;
        }

        private async Task<string> GenerateAndSaveRefreshToken(User user)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
            await context.SaveChangesAsync();
            return refreshToken;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.Username),
                 new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                  new Claim(ClaimTypes.Role,user.Roles)
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("Appsettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var user = await context.Users.FindAsync(request.UserId);
            if (user is null || user.RefreshToken != request.RefreshTokne ||
                     user.RefreshTokenExpiry < DateTime.UtcNow)
                return null;
            var token = new TokenResponseDto
            {
                AccessTokne = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshToken(user)
            };
            return token;
        }
    }
}
