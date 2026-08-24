using System.ComponentModel.DataAnnotations;
using EmployeeDeskBooking.Application.Auth;

namespace EmployeeDeskBooking.Web.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public bool IsDeactivatedError { get; set; }

    public static LoginViewModel FromFailure(SignInFailureReason reason) =>
        new()
        {
            ErrorMessage = reason switch
            {
                SignInFailureReason.DeactivatedAccount => AuthMessages.DeactivatedAccount,
                _ => AuthMessages.InvalidCredentials,
            },
            IsDeactivatedError = reason == SignInFailureReason.DeactivatedAccount,
        };
}
