using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Application.Users;
using EmployeeDeskBooking.Api.Models;
using EmployeeDeskBooking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDeskBooking.Api.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = AuthRoles.Admin)]
public class AdminUsersController(IUserAdminService userAdminService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        var users = await userAdminService.GetAllUsersAsync(cancellationToken);
        return Ok(users.Select(user => new AdminUserResponse
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            IsLastActiveAdmin = user.IsLastActiveAdmin,
        }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryParseRole(request.Role, out var role))
        {
            return BadRequest(new ErrorResponse { Message = "Role must be Admin or Employee." });
        }

        var result = await userAdminService.CreateUserAsync(
            request.Email,
            request.Name,
            role,
            request.Password,
            cancellationToken);

        if (!result.Succeeded)
        {
            return Failure(result);
        }

        return Created(string.Empty, new { UserId = result.UserId });
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Deactivate == true)
        {
            var deactivateResult = await userAdminService.DeactivateUserAsync(id, cancellationToken);
            if (!deactivateResult.Succeeded)
            {
                return Failure(deactivateResult);
            }

            return NoContent();
        }

        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Role))
        {
            return BadRequest(new ErrorResponse { Message = "Name, email, and role are required." });
        }

        if (!TryParseRole(request.Role, out var role))
        {
            return BadRequest(new ErrorResponse { Message = "Role must be Admin or Employee." });
        }

        var result = await userAdminService.UpdateUserAsync(
            id,
            request.Email,
            request.Name,
            role,
            cancellationToken);

        if (!result.Succeeded)
        {
            return Failure(result);
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/reset-password")]
    public async Task<IActionResult> ResetPassword(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await userAdminService.ResetPasswordAsync(id, cancellationToken);
        if (!result.Succeeded)
        {
            return StatusCode(
                result.FailureReason == UserMutationFailureReason.NotFound
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status409Conflict,
                new ErrorResponse
                {
                    Message = AdminUserFailureMessage.ForMutationFailure(result.FailureReason),
                });
        }

        return Ok(new ResetPasswordResponse { Password = result.PlaintextPassword! });
    }

    private IActionResult Failure(UserMutationResult result)
    {
        var statusCode = result.FailureReason switch
        {
            UserMutationFailureReason.InvalidEmail => StatusCodes.Status400BadRequest,
            UserMutationFailureReason.InvalidName => StatusCodes.Status400BadRequest,
            UserMutationFailureReason.InvalidPassword => StatusCodes.Status400BadRequest,
            UserMutationFailureReason.InvalidRole => StatusCodes.Status400BadRequest,
            UserMutationFailureReason.DuplicateEmail => StatusCodes.Status422UnprocessableEntity,
            UserMutationFailureReason.NotFound => StatusCodes.Status404NotFound,
            UserMutationFailureReason.LastActiveAdmin => StatusCodes.Status422UnprocessableEntity,
            UserMutationFailureReason.AlreadyInactive => StatusCodes.Status422UnprocessableEntity,
            UserMutationFailureReason.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest,
        };

        return StatusCode(statusCode, new ErrorResponse
        {
            Message = AdminUserFailureMessage.ForMutationFailure(result.FailureReason),
        });
    }

    private static bool TryParseRole(string? value, out UserRole role)
    {
        if (string.Equals(value, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            role = UserRole.Admin;
            return true;
        }

        if (string.Equals(value, "Employee", StringComparison.OrdinalIgnoreCase))
        {
            role = UserRole.Employee;
            return true;
        }

        role = default;
        return false;
    }
}

internal static class AdminUserFailureMessage
{
    public static string ForMutationFailure(UserMutationFailureReason reason) =>
        reason switch
        {
            UserMutationFailureReason.InvalidEmail => "Enter a valid email address.",
            UserMutationFailureReason.InvalidName => "Enter the user's name.",
            UserMutationFailureReason.InvalidPassword => "Password must be at least 8 characters.",
            UserMutationFailureReason.DuplicateEmail => "That email is already in use.",
            UserMutationFailureReason.NotFound => "User not found.",
            UserMutationFailureReason.LastActiveAdmin => "Cannot remove the last active Admin.",
            UserMutationFailureReason.AlreadyInactive => "Account is already inactive.",
            UserMutationFailureReason.Conflict => "Could not save the user. Try again.",
            _ => "Could not save the user.",
        };
}
