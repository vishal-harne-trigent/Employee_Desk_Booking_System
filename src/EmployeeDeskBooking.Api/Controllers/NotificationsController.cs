using System.Security.Claims;
using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Application.Notifications;
using EmployeeDeskBooking.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EmployeeDeskBooking.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize(Roles = AuthRoles.Employee)]
public class NotificationsController(
    INotificationPreferenceService notificationPreferenceService,
    IConfiguration configuration) : ControllerBase
{
    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences(CancellationToken cancellationToken)
    {
        var preferences = await notificationPreferenceService.GetPreferencesAsync(
            GetUserId(),
            cancellationToken);

        return Ok(new NotificationPreferencesResponse
        {
            PushOptIn = preferences.PushOptIn,
            HasSubscription = preferences.HasSubscription,
            VapidPublicKey = configuration["Vapid:PublicKey"],
        });
    }

    [HttpPatch("preferences")]
    public async Task<IActionResult> UpdatePreferences(
        [FromBody] UpdateNotificationPreferencesRequest request,
        CancellationToken cancellationToken)
    {
        if (request.PushOptIn == false)
        {
            await notificationPreferenceService.DisablePushAsync(GetUserId(), cancellationToken);
            return NoContent();
        }

        return BadRequest(new ErrorResponse
        {
            Message = "Use POST /api/notifications/push-subscription to enable push.",
        });
    }

    [HttpPost("push-subscription")]
    public async Task<IActionResult> SavePushSubscription(
        [FromBody] PushSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SubscriptionJson))
        {
            return BadRequest(new ErrorResponse { Message = "Subscription JSON is required." });
        }

        await notificationPreferenceService.EnablePushAsync(
            GetUserId(),
            request.SubscriptionJson,
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("push-subscription")]
    public async Task<IActionResult> RemovePushSubscription(CancellationToken cancellationToken)
    {
        await notificationPreferenceService.DisablePushAsync(GetUserId(), cancellationToken);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Signed-in user id is missing.");

        return Guid.Parse(value);
    }
}
