using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Users;

namespace Bookify.Application.Bookings.ReserveBooking;

internal sealed class BookingReservedDomainEventHandler(
        IBookingRepository bookingRepository,
        IUserRepository userRepository,
        IEmailService emailService) : IDomainEventHandler<BookingReservedDomainEvent>
{
    public async Task Handle(BookingReservedDomainEvent notification, CancellationToken cancellationToken)
    {
        Booking? booking = await bookingRepository.GetByIdReadOnlyAsync(
            notification.BookingId,
            cancellationToken);

        if(booking is null)
        {
            return;
        }

        User? user = await userRepository.GetByIdReadOnlyAsync(
            booking.UserId,
            cancellationToken);

        if (user is null)
        {
            return;
        }

        await emailService.SendAsync(
            user.Email,
            "Booking reserved!",
            "You have 10 minutes to confirm this booking.");
    }
}
