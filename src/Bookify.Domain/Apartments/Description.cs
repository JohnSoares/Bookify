using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;

public sealed record Description
{
    public const int MaxLength = 2000;

    private Description(string value) => Value = value;

    public string Value { get; }

    public static Result<Description> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<Description>(DescriptionErrors.Empty);
        }

        string normalizedValue = value.Trim();

        if (normalizedValue.Length > MaxLength)
        {
            return Result.Failure<Description>(DescriptionErrors.TooLong);
        }

        return new Description(normalizedValue);
    }
}
