using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users;

public sealed record LastName
{
    public const int MaxLength = 200;

    private LastName(string value) => Value = value;

    public string Value { get; }

    public static Result<LastName> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<LastName>(LastNameErrors.Empty);
        }

        string normalizedValue = value.Trim();

        if (normalizedValue.Length > MaxLength)
        {
            return Result.Failure<LastName>(LastNameErrors.TooLong);
        }

        return new LastName(normalizedValue);
    }
}
