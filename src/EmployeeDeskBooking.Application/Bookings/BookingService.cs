using EmployeeDeskBooking.Application.Desks;
using EmployeeDeskBooking.Application.Notifications;
using EmployeeDeskBooking.Domain.Entities;
using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Application.Bookings;

public sealed class BookingService(
    IOfficeClock officeClock,
    IDeskRepository deskRepository,
    IBookingRepository bookingRepository,
    IBookingNotificationService bookingNotificationService) : IBookingService
{
    public async Task<AvailabilityResult> GetAvailabilityAsync(
        Guid userId,
        DateOnly bookingDate,
        CancellationToken cancellationToken = default)
    {
        var validation = BookingDateRules.Validate(bookingDate, officeClock.Today);
        if (!validation.IsValid)
        {
            return AvailabilityResult.InvalidDate(validation.Failure);
        }

        var existing = await bookingRepository.GetConfirmedForUserOnDateAsync(
            userId,
            bookingDate,
            cancellationToken);

        EmployeeBookingSummary? existingSummary = existing is null
            ? null
            : new EmployeeBookingSummary(existing.Id, existing.Desk.DeskNumber);

        var activeDesks = await deskRepository.GetActiveDesksAsync(cancellationToken);
        var bookedDeskIds = await bookingRepository.GetBookedDeskIdsForDateAsync(
            bookingDate,
            cancellationToken);

        var desks = activeDesks
            .OrderBy(desk => desk.DeskNumber, StringComparer.OrdinalIgnoreCase)
            .Select(desk => new DeskAvailabilityItem(
                desk.Id,
                desk.DeskNumber,
                !bookedDeskIds.Contains(desk.Id)))
            .ToList();

        return new AvailabilityResult(true, DateValidationFailure.None, desks, existingSummary);
    }

    public async Task<CreateBookingResult> CreateBookingAsync(
        Guid userId,
        Guid deskId,
        DateOnly bookingDate,
        CancellationToken cancellationToken = default)
    {
        var validation = BookingDateRules.Validate(bookingDate, officeClock.Today);
        if (!validation.IsValid)
        {
            return CreateBookingResult.Failed(CreateBookingFailureReason.InvalidDate);
        }

        var existing = await bookingRepository.GetConfirmedForUserOnDateAsync(
            userId,
            bookingDate,
            cancellationToken);

        if (existing is not null)
        {
            return CreateBookingResult.Failed(CreateBookingFailureReason.EmployeeAlreadyBooked);
        }

        var desk = await deskRepository.FindByIdAsync(deskId, cancellationToken);
        if (desk is null)
        {
            return CreateBookingResult.Failed(CreateBookingFailureReason.DeskNotFound);
        }

        if (desk.Status != DeskStatus.Active)
        {
            return CreateBookingResult.Failed(CreateBookingFailureReason.DeskInactive);
        }

        var bookedDeskIds = await bookingRepository.GetBookedDeskIdsForDateAsync(
            bookingDate,
            cancellationToken);

        if (bookedDeskIds.Contains(deskId))
        {
            return CreateBookingResult.Failed(CreateBookingFailureReason.DeskAlreadyBooked);
        }

        var now = officeClock.Now;
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DeskId = deskId,
            BookingDate = bookingDate,
            Status = BookingStatus.Confirmed,
            CreatedAt = now,
            UpdatedAt = now,
        };

        await bookingRepository.AddAsync(booking, cancellationToken);
        var saved = await bookingRepository.TrySaveChangesAsync(cancellationToken);
        if (!saved)
        {
            return CreateBookingResult.Failed(CreateBookingFailureReason.Conflict);
        }

        await bookingNotificationService.SendConfirmationAsync(booking.Id, cancellationToken);
        return CreateBookingResult.Success(booking.Id, desk.DeskNumber);
    }

    public async Task<IReadOnlyList<MyBookingItem>> GetMyBookingsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var today = officeClock.Today;
        var bookings = await bookingRepository.GetAllForUserAsync(userId, cancellationToken);

        return bookings
            .OrderByDescending(booking => booking.BookingDate)
            .Select(booking => new MyBookingItem(
                booking.Id,
                booking.BookingDate,
                booking.Desk.DeskNumber,
                booking.Status,
                BookingCancellationRules.CanCancel(booking.Status, booking.BookingDate, today)))
            .ToList();
    }

    public async Task<CancelBookingResult> CancelBookingAsync(
        Guid userId,
        Guid bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdForUserAsync(
            bookingId,
            userId,
            cancellationToken);

        if (booking is null)
        {
            return CancelBookingResult.Failed(CancelBookingFailureReason.NotFound);
        }

        if (!BookingCancellationRules.CanCancel(
                booking.Status,
                booking.BookingDate,
                officeClock.Today))
        {
            return CancelBookingResult.Failed(CancelBookingFailureReason.NotCancellable);
        }

        var now = officeClock.Now;
        booking.Status = BookingStatus.Cancelled;
        booking.CancelledAt = now;
        booking.CancelledById = userId;
        booking.UpdatedAt = now;

        var saved = await bookingRepository.TrySaveChangesAsync(cancellationToken);
        if (!saved)
        {
            return CancelBookingResult.Failed(CancelBookingFailureReason.Conflict);
        }

        await bookingNotificationService.SendCancellationAsync(bookingId, cancellationToken);
        return CancelBookingResult.Success();
    }

    public async Task<IReadOnlyList<AdminBookingItem>> GetAllBookingsAsync(
        AdminBookingFilters filters,
        CancellationToken cancellationToken = default)
    {
        var today = officeClock.Today;
        var bookings = await bookingRepository.GetAllAsync(
            filters.BookingDate,
            filters.Status,
            cancellationToken);

        return bookings
            .OrderByDescending(booking => booking.BookingDate)
            .ThenBy(booking => booking.User.Email, StringComparer.OrdinalIgnoreCase)
            .Select(booking => new AdminBookingItem(
                booking.Id,
                booking.BookingDate,
                booking.Desk.DeskNumber,
                booking.User.Email,
                booking.Status,
                BookingCancellationRules.CanCancel(booking.Status, booking.BookingDate, today)))
            .ToList();
    }

    public async Task<CancelBookingResult> CancelBookingAsAdminAsync(
        Guid adminUserId,
        Guid bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken);

        if (booking is null)
        {
            return CancelBookingResult.Failed(CancelBookingFailureReason.NotFound);
        }

        if (!BookingCancellationRules.CanCancel(
                booking.Status,
                booking.BookingDate,
                officeClock.Today))
        {
            return CancelBookingResult.Failed(CancelBookingFailureReason.NotCancellable);
        }

        var now = officeClock.Now;
        booking.Status = BookingStatus.Cancelled;
        booking.CancelledAt = now;
        booking.CancelledById = adminUserId;
        booking.UpdatedAt = now;

        var saved = await bookingRepository.TrySaveChangesAsync(cancellationToken);
        if (!saved)
        {
            return CancelBookingResult.Failed(CancelBookingFailureReason.Conflict);
        }

        await bookingNotificationService.SendCancellationAsync(bookingId, cancellationToken);
        return CancelBookingResult.Success();
    }
}
