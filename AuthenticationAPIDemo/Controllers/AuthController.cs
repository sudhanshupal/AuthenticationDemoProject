using AuthenticationAPIDemo.Entities;
using AuthenticationAPIDemo.Model;
using AuthenticationAPIDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationAPIDemo.Controllers
{

    [Route("api/[controller]")]
    [ApiController]


    public class AuthController : ControllerBase
    {
        private readonly IAuthService service;
        public AuthController(IAuthService service)
        {
            this.service = service;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User?>> Register(UserDto request)
        {
            var user = await service.RegisterAsync(request);
            if (user is null) 
                return BadRequest("Username already exist");
                return Ok(user);
           
        }
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            var token = await service.LoginAsync(request);
            if (token is null)
                return BadRequest("Username/password is wrong");

            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto?>> RefreshToken(RefreshTokenRequestDto request)
        {
            var token = await service.RefreshTokenAsync(request);
            if (token is null)
                return BadRequest("Invalid /Expired Token");

            return Ok(token);
        }

        [HttpGet("Auth-endpoint")]
        [Authorize]
        public ActionResult AuthCheck()
        {
            return Ok();
        }
        [HttpGet("Admin-endpoint")]
        [Authorize(Roles = "Admin")]
        public ActionResult AdminCheck()
        {
            return Ok();
        }
    }
}