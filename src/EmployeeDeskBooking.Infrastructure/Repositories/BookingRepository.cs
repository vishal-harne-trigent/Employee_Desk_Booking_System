using EmployeeDeskBooking.Application.Bookings;
using EmployeeDeskBooking.Application.Desks;
using EmployeeDeskBooking.Domain.Entities;
using EmployeeDeskBooking.Domain.Enums;
using EmployeeDeskBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDeskBooking.Infrastructure.Repositories;

public sealed class DeskRepository(AppDbContext dbContext) : IDeskRepository
{
    public async Task<IReadOnlyList<Desk>> GetActiveDesksAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Desks
            .AsNoTracking()
            .Where(desk => desk.Status == DeskStatus.Active)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Desk>> GetAllDesksAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Desks
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<Desk?> FindByIdAsync(
        Guid deskId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Desks
            .AsNoTracking()
            .FirstOrDefaultAsync(desk => desk.Id == deskId, cancellationToken);

    public async Task<Desk?> FindByIdTrackedAsync(
        Guid deskId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Desks
            .FirstOrDefaultAsync(desk => desk.Id == deskId, cancellationToken);

    public async Task<Desk?> FindByNormalizedNumberAsync(
        string normalizedDeskNumber,
        CancellationToken cancellationToken = default) =>
        await dbContext.Desks
            .AsNoTracking()
            .FirstOrDefaultAsync(
                desk => desk.DeskNumberNormalized == normalizedDeskNumber,
                cancellationToken);

    public Task AddAsync(Desk desk, CancellationToken cancellationToken = default)
    {
        dbContext.Desks.Add(desk);
        return Task.CompletedTask;
    }

    public async Task<int> CountConfirmedBookingsOnOrAfterAsync(
        Guid deskId,
        DateOnly fromDate,
        CancellationToken cancellationToken = default) =>
        await dbContext.Bookings
            .AsNoTracking()
            .CountAsync(
                booking =>
                    booking.DeskId == deskId &&
                    booking.Status == BookingStatus.Confirmed &&
                    booking.BookingDate >= fromDate,
                cancellationToken);

    public async Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }
}

public sealed class BookingRepository(AppDbContext dbContext) : IBookingRepository
{
    public async Task<Booking?> GetConfirmedForUserOnDateAsync(
        Guid userId,
        DateOnly bookingDate,
        CancellationToken cancellationToken = default) =>
        await dbContext.Bookings
            .AsNoTracking()
            .Include(booking => booking.Desk)
            .FirstOrDefaultAsync(
                booking =>
                    booking.UserId == userId &&
                    booking.BookingDate == bookingDate &&
                    booking.Status == BookingStatus.Confirmed,
                cancellationToken);

    public async Task<HashSet<Guid>> GetBookedDeskIdsForDateAsync(
        DateOnly bookingDate,
        CancellationToken cancellationToken = default)
    {
        var deskIds = await dbContext.Bookings
            .AsNoTracking()
            .Where(booking =>
                booking.BookingDate == bookingDate &&
                booking.Status == BookingStatus.Confirmed)
            .Select(booking => booking.DeskId)
            .ToListAsync(cancellationToken);

        return deskIds.ToHashSet();
    }

    public Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        dbContext.Bookings.Add(booking);
        return Task.CompletedTask;
    }

    public async Task<Booking?> GetByIdForUserAsync(
        Guid bookingId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Bookings
            .Include(booking => booking.Desk)
            .FirstOrDefaultAsync(
                booking => booking.Id == bookingId && booking.UserId == userId,
                cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetAllForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Bookings
            .AsNoTracking()
            .Include(booking => booking.Desk)
            .Where(booking => booking.UserId == userId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetAllAsync(
        DateOnly? bookingDate,
        BookingStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Bookings
            .AsNoTracking()
            .Include(booking => booking.Desk)
            .Include(booking => booking.User)
            .AsQueryable();

        if (bookingDate is not null)
        {
            query = query.Where(booking => booking.BookingDate == bookingDate);
        }

        if (status is not null)
        {
            query = query.Where(booking => booking.Status == status);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Booking?> GetByIdAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Bookings
            .Include(booking => booking.Desk)
            .Include(booking => booking.User)
            .FirstOrDefaultAsync(booking => booking.Id == bookingId, cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetConfirmedBookingsForDateAsync(
        DateOnly bookingDate,
        CancellationToken cancellationToken = default) =>
        await dbContext.Bookings
            .AsNoTracking()
            .Include(booking => booking.Desk)
            .Include(booking => booking.User)
            .Where(booking =>
                booking.BookingDate == bookingDate &&
                booking.Status == BookingStatus.Confirmed)
            .ToListAsync(cancellationToken);

    public async Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }
}
