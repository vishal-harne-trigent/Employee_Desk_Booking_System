using System.Security.Claims;
using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Application.Notifications;
using EmployeeDeskBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EmployeeDeskBooking.Web.Controllers;

[Authorize(Roles = AuthRoles.Employee)]
public class NotificationSettingsController(
    INotificationPreferenceService notificationPreferenceService,
    IConfiguration configuration) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var preferences = await notificationPreferenceService.GetPreferencesAsync(
            GetUserId(),
            cancellationToken);

        return View(ToViewModel(preferences));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnablePush(
        string subscriptionJson,
        CancellationToken cancellationToken)
    {
        try
        {
            await notificationPreferenceService.EnablePushAsync(
                GetUserId(),
                subscriptionJson,
                cancellationToken);

            var preferences = await notificationPreferenceService.GetPreferencesAsync(
                GetUserId(),
                cancellationToken);

            var model = ToViewModel(preferences);
            model.SuccessMessage = "Browser push notifications are enabled.";
            return View("Index", model);
        }
        catch (Exception)
        {
            return await ErrorViewAsync(
                "Could not save push preferences. Try again.",
                cancellationToken);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DisablePush(CancellationToken cancellationToken)
    {
        await notificationPreferenceService.DisablePushAsync(GetUserId(), cancellationToken);

        var preferences = await notificationPreferenceService.GetPreferencesAsync(
            GetUserId(),
            cancellationToken);

        var model = ToViewModel(preferences);
        model.SuccessMessage = "Browser push notifications are disabled.";
        return View("Index", model);
    }

    private async Task<IActionResult> ErrorViewAsync(
        string message,
        CancellationToken cancellationToken)
    {
        var preferences = await notificationPreferenceService.GetPreferencesAsync(
            GetUserId(),
            cancellationToken);

        var model = ToViewModel(preferences);
        model.ErrorMessage = message;
        return View("Index", model);
    }

    private NotificationSettingsViewModel ToViewModel(NotificationPreferenceDto preferences) =>
        new()
        {
            PushOptIn = preferences.PushOptIn,
            HasSubscription = preferences.HasSubscription,
            VapidPublicKey = configuration["Vapid:PublicKey"],
        };

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Signed-in user id is missing.");

        return Guid.Parse(value);
    }
}
