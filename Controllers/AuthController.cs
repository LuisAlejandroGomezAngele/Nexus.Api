using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nexus.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Nexus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Validación básica simulada. Aquí conectas con tu BD real.
            if (request.Username != "admin" || request.Password != "1234")
                return Unauthorized(new { ok = false, message = "Credenciales inválidas" });

            var token = GenerateJwtToken(request.Username);
            return Ok(new { ok = true, token });
        }

        private string GenerateJwtToken(string username)
        {
            var jwtKey = _config["Jwt:Key"]!;
            var jwtIssuer = _config["Jwt:Issuer"]!;
            var expireMinutes = int.Parse(_config["Jwt:ExpireMinutes"] ?? "60");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin") // puedes agregar roles
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("verify")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult VerifyToken()
        {
            return Ok(new
            {
                ok = true,
                user = User.Identity?.Name ?? "Usuario autenticado",
                message = "Token válido"
            });
        }
    }
}
