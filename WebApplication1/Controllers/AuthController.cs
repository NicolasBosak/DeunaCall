using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DeunaCall.Data;
using DeunaCall.Data.Models;
using DeunaCall.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration)
    {
        _userManager = userManager;
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var role = userRoles.FirstOrDefault() ?? "Nurse";

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = GenerateToken(authClaims);
            return Ok(new AuthResponseDto { Token = new JwtSecurityTokenHandler().WriteToken(token), Role = role });
        }
        return Unauthorized();
    }

    [HttpPost("login-patient")]
    public async Task<IActionResult> PatientLogin([FromBody] PatientLoginDto model)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.AccessCode == model.AccessCode);
        if (patient == null) return Unauthorized("Invalid access code.");

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, patient.Id.ToString()),
            new Claim("PatientId", patient.Id.ToString()),
            new Claim(ClaimTypes.Role, "Patient"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = GenerateToken(authClaims);
        return Ok(new AuthResponseDto { Token = new JwtSecurityTokenHandler().WriteToken(token), Role = "Patient" });
    }

    private JwtSecurityToken GenerateToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: DateTime.Now.AddHours(12),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return token;
    }
}
