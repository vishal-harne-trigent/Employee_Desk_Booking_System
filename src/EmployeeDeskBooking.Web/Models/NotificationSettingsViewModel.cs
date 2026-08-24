namespace EmployeeDeskBooking.Web.Models;

public class NotificationSettingsViewModel
{
    public bool PushOptIn { get; set; }

    public bool HasSubscription { get; set; }

    public string? VapidPublicKey { get; set; }

    public bool PushConfigured => !string.IsNullOrWhiteSpace(VapidPublicKey);

    public string? SuccessMessage { get; set; }

    public string? ErrorMessage { get; set; }
}
