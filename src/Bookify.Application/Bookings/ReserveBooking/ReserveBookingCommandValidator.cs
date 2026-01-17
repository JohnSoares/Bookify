using FluentValidation;

namespace Bookify.Application.Bookings.ReserveBooking;

internal sealed class ReserveBookingCommandValidator : AbstractValidator<ReserveBookingCommand>
{
    public ReserveBookingCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.ApartmentId)
            .NotEmpty().WithMessage("ApartmentId is required.");

        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate).WithMessage("StartDate must be before EndDate.");
    }
}
