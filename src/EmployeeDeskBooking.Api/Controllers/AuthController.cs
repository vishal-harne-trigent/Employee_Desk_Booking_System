using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeDeskBooking.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, IConfiguration configuration) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Unauthorized(new ErrorResponse { Message = AuthMessages.InvalidCredentials });
        }

        var result = await authService.SignInAsync(request.Email, request.Password, cancellationToken);
        if (!result.Succeeded || result.User is null)
        {
            var message = result.FailureReason == SignInFailureReason.DeactivatedAccount
                ? AuthMessages.DeactivatedAccount
                : AuthMessages.InvalidCredentials;

            var status = result.FailureReason == SignInFailureReason.DeactivatedAccount
                ? StatusCodes.Status403Forbidden
                : StatusCodes.Status401Unauthorized;

            return StatusCode(status, new ErrorResponse { Message = message });
        }

        var expiryMinutes = configuration.GetValue("Jwt:ExpiryMinutes", 60);
        var token = CreateToken(result.User, expiryMinutes);

        return Ok(new LoginResponse
        {
            Token = token,
            ExpiresInMinutes = expiryMinutes,
            Email = result.User.Email,
            Name = result.User.Name,
            Role = result.User.Role,
        });
    }

    private string CreateToken(SignInUser user, int expiryMinutes)
    {
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
