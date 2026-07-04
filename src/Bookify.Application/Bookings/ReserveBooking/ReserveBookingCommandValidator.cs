using FluentValidation;

namespace Bookify.Application.Bookings.ReserveBooking;

internal sealed class ReserveBookingCommandValidator : AbstractValidator<ReserveBookingCommand>
{
    public ReserveBookingCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithErrorCode(BookingErrorCodes.ReserveBooking.MissingUserId);

        RuleFor(x => x.ApartmentId)
            .NotEmpty().WithErrorCode(BookingErrorCodes.ReserveBooking.MissingApartmentId);

        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate).WithErrorCode(BookingErrorCodes.ReserveBooking.InvalidDates);
    }
}
