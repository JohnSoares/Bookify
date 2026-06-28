using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;

public sealed record Name
{
    public const int MaxLength = 200;

    private Name(string value) => Value = value;

    public string Value { get; }

    public static Result<Name> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<Name>(NameErrors.Empty);
        }

        string normalizedValue = value.Trim();

        if (normalizedValue.Length > MaxLength)
        {
            return Result.Failure<Name>(NameErrors.TooLong);
        }

        return new Name(normalizedValue);
    }
}
