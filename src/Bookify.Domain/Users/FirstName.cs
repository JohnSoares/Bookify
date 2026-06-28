using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users;

public sealed record FirstName
{
    public const int MaxLength = 200;

    private FirstName(string value) => Value = value;

    public string Value { get; }

    public static Result<FirstName> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<FirstName>(FirstNameErrors.Empty);
        }

        string normalizedValue = value.Trim();

        if (normalizedValue.Length > MaxLength)
        {
            return Result.Failure<FirstName>(FirstNameErrors.TooLong);
        }

        return new FirstName(normalizedValue);
    }
}
