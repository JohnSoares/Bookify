using Bookify.Application.Bookings.RejectBooking;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using FluentAssertions;
using NSubstitute;

namespace Bookify.Application.UnitTests.Bookings;

public class RejectBookingTests
{
    private static readonly DateTime UtcNow = new(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private static readonly RejectBookingCommand Command = new(Guid.NewGuid());

    private readonly IBookingRepository _bookingRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly RejectBookingCommandCommandHandler _handler;

    public RejectBookingTests()
    {
        IDateTimeProvider dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
        _bookingRepositoryMock = Substitute.For<IBookingRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        dateTimeProviderMock.UtcNow.Returns(UtcNow);

        _handler = new RejectBookingCommandCommandHandler(
            dateTimeProviderMock,
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
    public async Task Handle_Should_ReturnFailure_WhenBookingIsNotReserved()
    {
        // Arrange
        Booking booking = BookingData.CreateConfirmed();

        _bookingRepositoryMock
            .GetByIdAsync(Command.BookingId, Arg.Any<CancellationToken>())
            .Returns(booking);

        // Act
        Result result = await _handler.Handle(Command, default);

        // Assert
        result.Error.Should().Be(BookingErrors.NotReserved);
        await _unitOfWorkMock.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenBookingIsRejected()
    {
        // Arrange
        Booking booking = BookingData.CreateReserved();

        _bookingRepositoryMock
            .GetByIdAsync(Command.BookingId, Arg.Any<CancellationToken>())
            .Returns(booking);

        // Act
        Result result = await _handler.Handle(Command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Rejected);
        booking.RejectedOnUtc.Should().Be(UtcNow);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
