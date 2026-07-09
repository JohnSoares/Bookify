using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Shared;
using Bookify.Domain.UnitTests.Apartments;
using Bookify.Domain.UnitTests.Infrastructure;
using FluentAssertions;

namespace Bookify.Domain.UnitTests.Bookings;

public class BookingStateTransitionTests : BaseTest
{
    private static readonly DateTime UtcNow = new(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Confirm_Should_ChangeStatusAndRaiseDomainEvent_WhenBookingIsReserved()
    {
        // Arrange
        Booking booking = CreateReservedBooking();

        // Act
        Result result = booking.Confirm(UtcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Confirmed);
        booking.ConfirmedOnUtc.Should().Be(UtcNow);

        BookingConfirmedDomainEvent domainEvent = AssertDomainEventWasPublished<BookingConfirmedDomainEvent>(booking);
        domainEvent.BookingId.Should().Be(booking.Id);
    }

    [Fact]
    public void Confirm_Should_ReturnFailure_WhenBookingIsNotReserved()
    {
        // Arrange
        Booking booking = CreateConfirmedBooking();

        // Act
        Result result = booking.Confirm(UtcNow);

        // Assert
        result.Error.Should().Be(BookingErrors.NotReserved);
    }

    [Fact]
    public void Reject_Should_ChangeStatusAndRaiseDomainEvent_WhenBookingIsReserved()
    {
        // Arrange
        Booking booking = CreateReservedBooking();

        // Act
        Result result = booking.Reject(UtcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Rejected);
        booking.RejectedOnUtc.Should().Be(UtcNow);

        BookingRejectedDomainEvent domainEvent = AssertDomainEventWasPublished<BookingRejectedDomainEvent>(booking);
        domainEvent.BookingId.Should().Be(booking.Id);
    }

    [Fact]
    public void Reject_Should_ReturnFailure_WhenBookingIsNotReserved()
    {
        // Arrange
        Booking booking = CreateConfirmedBooking();

        // Act
        Result result = booking.Reject(UtcNow);

        // Assert
        result.Error.Should().Be(BookingErrors.NotReserved);
    }

    [Fact]
    public void Complete_Should_ChangeStatusAndRaiseDomainEvent_WhenBookingIsConfirmed()
    {
        // Arrange
        Booking booking = CreateConfirmedBooking();

        // Act
        Result result = booking.Complete(UtcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Completed);
        booking.CompletedOnUtc.Should().Be(UtcNow);

        BookingCompletedDomainEvent domainEvent = AssertDomainEventWasPublished<BookingCompletedDomainEvent>(booking);
        domainEvent.BookingId.Should().Be(booking.Id);
    }

    [Fact]
    public void Complete_Should_ReturnFailure_WhenBookingIsNotConfirmed()
    {
        // Arrange
        Booking booking = CreateReservedBooking();

        // Act
        Result result = booking.Complete(UtcNow);

        // Assert
        result.Error.Should().Be(BookingErrors.NotConfirmed);
    }

    [Fact]
    public void Cancel_Should_ChangeStatusAndRaiseDomainEvent_WhenBookingIsConfirmedAndNotStarted()
    {
        // Arrange
        Booking booking = CreateConfirmedBooking();
        var cancellationDate = booking.Duration.Start.AddDays(-1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        // Act
        Result result = booking.Cancel(cancellationDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Cancelled);
        booking.CancelledOnUtc.Should().Be(cancellationDate);

        BookingCancelledDomainEvent domainEvent = AssertDomainEventWasPublished<BookingCancelledDomainEvent>(booking);
        domainEvent.BookingId.Should().Be(booking.Id);
    }

    [Fact]
    public void Cancel_Should_ReturnFailure_WhenBookingIsNotConfirmed()
    {
        // Arrange
        Booking booking = CreateReservedBooking();

        // Act
        Result result = booking.Cancel(UtcNow);

        // Assert
        result.Error.Should().Be(BookingErrors.NotConfirmed);
    }

    [Fact]
    public void Cancel_Should_ReturnFailure_WhenBookingAlreadyStarted()
    {
        // Arrange
        Booking booking = CreateConfirmedBooking();
        var cancellationDate = booking.Duration.Start.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        // Act
        Result result = booking.Cancel(cancellationDate);

        // Assert
        result.Error.Should().Be(BookingErrors.AlreadyStarted);
    }

    private static Booking CreateReservedBooking()
    {
        var price = new Money(10.0m, Currency.USD);
        DateRange duration = DateRange
            .Create(new DateOnly(2024, 1, 10), new DateOnly(2024, 1, 20))
            .Value;
        Apartment apartment = ApartmentData.Create(price);

        return Booking.Reserve(apartment, Guid.NewGuid(), duration, UtcNow);
    }

    private static Booking CreateConfirmedBooking()
    {
        Booking booking = CreateReservedBooking();
        booking.Confirm(UtcNow);

        return booking;
    }
}
