using System.Security.Claims;
using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDeskBooking.Web.Controllers;

public class AccountController(IAuthService authService) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectForRole(User);
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await authService.SignInAsync(model.Email, model.Password);
        if (!result.Succeeded || result.User is null)
        {
            var failureModel = LoginViewModel.FromFailure(result.FailureReason);
            failureModel.Email = model.Email;

            return View(failureModel);
        }

        await SignInUserAsync(result.User);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectForRole(result.User.Role);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    private async Task SignInUserAsync(SignInUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, user.Role),
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = true,
            });
    }

    private IActionResult RedirectForRole(ClaimsPrincipal principal)
    {
        if (principal.IsInRole(AuthRoles.Admin))
        {
            return RedirectToAction("Index", "Bookings", new { area = "Admin" });
        }

        return RedirectToAction("Index", "Book");
    }

    private IActionResult RedirectForRole(string role)
    {
        if (role == AuthRoles.Admin)
        {
            return RedirectToAction("Index", "Bookings", new { area = "Admin" });
        }

        return RedirectToAction("Index", "Book");
    }
}
