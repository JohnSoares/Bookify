using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;

namespace Bookify.Application.Bookings.ReserveBooking;

internal sealed class ReserveBookingCommandHandler(
        IUserRepository userRepository,
        IApartmentRepository apartmentRepository,
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider) : ICommandHandler<ReserveBookingCommand, Guid>
{
    public async Task<Result<Guid>> Handle(ReserveBookingCommand request, CancellationToken cancellationToken)
    {
        User user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(request.UserId));
        }

        Apartment apartment = await apartmentRepository.GetByIdAsync(request.ApartmentId, cancellationToken);

        if(apartment is null)
        {
            return Result.Failure<Guid>(ApartmentErrors.NotFound(request.ApartmentId));
        }

        Result<DateRange> durationResult = DateRange.Create(
            request.StartDate,
            request.EndDate);

        if (durationResult.IsFailure)
        {
            return Result.Failure<Guid>(durationResult.Error);
        }

        DateRange duration = durationResult.Value;

        if(await bookingRepository.IsOverlappingAsync(apartment, duration, cancellationToken))
        {
            return Result.Failure<Guid>(BookingErrors.Overlap);
        }
        try
        {
            var booking = Booking.Reserve(
                apartment,
                user.Id,
                duration,
                dateTimeProvider.UtcNow);

            bookingRepository.Insert(booking);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return booking.Id;
        }
        catch (ConcurrencyException)
        {
            return Result.Failure<Guid>(BookingErrors.Overlap);
        }
    }
}
