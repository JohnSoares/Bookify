using Bookify.Application.Reviews.AddReview;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Bookify.Domain.Reviews;
using FluentAssertions;
using NSubstitute;

namespace Bookify.Application.UnitTests.Reviews;

public class AddReviewTests
{
    private static readonly DateTime UtcNow = new(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private static readonly AddReviewCommand Command = new(Guid.NewGuid(), 5, "Great stay");

    private readonly IBookingRepository _bookingRepositoryMock;
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly AddReviewCommandHandler _handler;

    public AddReviewTests()
    {
        _bookingRepositoryMock = Substitute.For<IBookingRepository>();
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        IDateTimeProvider dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
        dateTimeProviderMock.UtcNow.Returns(UtcNow);

        _handler = new AddReviewCommandHandler(
            _bookingRepositoryMock,
            _reviewRepositoryMock,
            _unitOfWorkMock,
            dateTimeProviderMock);
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
    public async Task Handle_Should_ReturnFailure_WhenRatingIsInvalid()
    {
        // Arrange
        Booking booking = Bookings.BookingData.CreateCompleted();
        var command = new AddReviewCommand(Command.BookingId, 6, Command.Comment);

        _bookingRepositoryMock
            .GetByIdAsync(command.BookingId, Arg.Any<CancellationToken>())
            .Returns(booking);

        // Act
        Result result = await _handler.Handle(command, default);

        // Assert
        result.Error.Should().Be(ReviewErrors.Invalid);
        _reviewRepositoryMock.DidNotReceiveWithAnyArgs().Insert(default!);
        await _unitOfWorkMock.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenBookingIsNotEligible()
    {
        // Arrange
        Booking booking = Bookings.BookingData.CreateReserved();

        _bookingRepositoryMock
            .GetByIdAsync(Command.BookingId, Arg.Any<CancellationToken>())
            .Returns(booking);

        // Act
        Result result = await _handler.Handle(Command, default);

        // Assert
        result.Error.Should().Be(ReviewErrors.NotEligible);
        _reviewRepositoryMock.DidNotReceiveWithAnyArgs().Insert(default!);
        await _unitOfWorkMock.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_Should_InsertReview_WhenReviewIsAdded()
    {
        // Arrange
        Booking booking = Bookings.BookingData.CreateCompleted();

        _bookingRepositoryMock
            .GetByIdAsync(Command.BookingId, Arg.Any<CancellationToken>())
            .Returns(booking);

        // Act
        Result result = await _handler.Handle(Command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _reviewRepositoryMock.Received(1).Insert(Arg.Is<Review>(
            review =>
                review.BookingId == booking.Id &&
                review.ApartmentId == booking.ApartmentId &&
                review.UserId == booking.UserId &&
                review.Rating.Value == Command.Rating &&
                review.Comment.Value == Command.Comment &&
                review.CreatedOnUtc == UtcNow));
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
