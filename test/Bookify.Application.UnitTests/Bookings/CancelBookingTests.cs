using Bookify.Application.Bookings.CancelBooking;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using FluentAssertions;
using NSubstitute;

namespace Bookify.Application.UnitTests.Bookings;

public class CancelBookingTests
{
    private static readonly CancelBookingCommand Command = new(Guid.NewGuid());

    private readonly IDateTimeProvider _dateTimeProviderMock;
    private readonly IBookingRepository _bookingRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CancelBookingCommandHandler _handler;

    public CancelBookingTests()
    {
        _dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
        _bookingRepositoryMock = Substitute.For<IBookingRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _handler = new CancelBookingCommandHandler(
            _dateTimeProviderMock,
            _bookingRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenBookingIsNull()
    {
        // Arrange
        _bookingRepositoryMock
            .GetByIdAsync(Command.BookingId, Arg.Any<CancellationToken>())
            .Returns((Booking?)null);

        // Act
        Result result = await _handler.Handle(Command, default);

        // Assert
        result.Error.Should().Be(BookingErrors.NotFound(Command.BookingId));
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenBookingIsNotConfirmed()
    {
        // Arrange
        Booking booking = BookingData.CreateReserved();

        _bookingRepositoryMock
            .GetByIdAsync(Command.BookingId, Arg.Any<CancellationToken>())
            .Returns(booking);

        // Act
        Result result = await _handler.Handle(Command, default);

        // Assert
        result.Error.Should().Be(BookingErrors.NotConfirmed);
        await _unitOfWorkMock.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenBookingAlreadyStarted()
    {
        // Arrange
        Booking booking = BookingData.CreateConfirmed();
        var utcNow = booking.Duration.Start.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        _dateTimeProviderMock.UtcNow.Returns(utcNow);

        _bookingRepositoryMock
            .GetByIdAsync(Command.BookingId, Arg.Any<CancellationToken>())
            .Returns(booking);

        // Act
        Result result = await _handler.Handle(Command, default);

        // Assert
        result.Error.Should().Be(BookingErrors.AlreadyStarted);
        await _unitOfWorkMock.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenBookingIsCancelled()
    {
        // Arrange
        Booking booking = BookingData.CreateConfirmed();
        var utcNow = booking.Duration.Start.AddDays(-1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        _dateTimeProviderMock.UtcNow.Returns(utcNow);

        _bookingRepositoryMock
            .GetByIdAsync(Command.BookingId, Arg.Any<CancellationToken>())
            .Returns(booking);

        // Act
        Result result = await _handler.Handle(Command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Cancelled);
        booking.CancelledOnUtc.Should().Be(utcNow);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
