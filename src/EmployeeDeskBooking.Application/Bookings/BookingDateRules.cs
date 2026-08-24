using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Application.Bookings;

public static class BookingDateRules
{
    public const int MaxDaysAhead = 30;

    public static DateValidationResult Validate(DateOnly date, DateOnly today)
    {
        if (date < today)
        {
            return DateValidationResult.Invalid(DateValidationFailure.BeforeToday);
        }

        if (date > today.AddDays(MaxDaysAhead))
        {
            return DateValidationResult.Invalid(DateValidationFailure.AfterWindow);
        }

        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            return DateValidationResult.Invalid(DateValidationFailure.Weekend);
        }

        return DateValidationResult.Valid();
    }
}

public static class BookingCancellationRules
{
    public static bool CanCancel(BookingStatus status, DateOnly bookingDate, DateOnly today) =>
        status == BookingStatus.Confirmed && bookingDate >= today;
}

public enum DateValidationFailure
{
    None = 0,
    BeforeToday = 1,
    AfterWindow = 2,
    Weekend = 3,
}

public sealed record DateValidationResult(bool IsValid, DateValidationFailure Failure)
{
    public static DateValidationResult Valid() => new(true, DateValidationFailure.None);

    public static DateValidationResult Invalid(DateValidationFailure failure) =>
        new(false, failure);
}
