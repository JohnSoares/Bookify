using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories;

internal sealed class BookingRepository : Repository<Booking>, IBookingRepository
{
    private static readonly BookingStatus[] ActiveBookingStatuses =
    {
        BookingStatus.Reserved,
        BookingStatus.Confirmed,
        BookingStatus.Completed
    };

    public BookingRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<bool> IsOverlappingAsync(
        Apartment apartment, 
        DateRange duration, 
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Booking>()
            .AnyAsync(
                booking =>
                    booking.ApartmentId == apartment.Id &&
                    ActiveBookingStatuses.Contains(booking.Status) &&
                    booking.Duration.Start <= duration.End &&
                    booking.Duration.End >= duration.Start,
                cancellationToken);
    }
}

