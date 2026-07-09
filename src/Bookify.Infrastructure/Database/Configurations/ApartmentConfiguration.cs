using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Database.Configurations;

internal sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.ToTable("apartments", tableBuilder =>
            tableBuilder.HasCheckConstraint(
                "ck_apartments_amenities_valid",
                $"amenities <@ ARRAY[{GetAllowedAmenities()}]::text[]"));

        builder.HasKey(apartment => apartment.Id);

        builder.OwnsOne(apartment => apartment.Address);

        builder.Property(apartment => apartment.Name)
            .HasMaxLength(Name.MaxLength)
            .HasConversion(name => name.Value, value => Name.Create(value).Value);

        builder.Property(apartment => apartment.Description)
            .HasMaxLength(Description.MaxLength)
            .HasConversion(
                description => description.Value,
                value => Description.Create(value).Value);

        builder.OwnsOne(apartment => apartment.Price, priceBuilder => priceBuilder.Property(money => money.Currency)
            .HasConversion(currency => currency.Code, code => Currency.FromCode(code)));

        builder.OwnsOne(apartment => apartment.CleaningFee, priceBuilder => priceBuilder.Property(money => money.Currency)
            .HasConversion(currency => currency.Code, code => Currency.FromCode(code)));

        builder.Property(apartment => apartment.Amenities)
            .HasColumnType("text[]")
            .HasConversion(
                amenities => amenities.Select(amenity => amenity.ToString()).ToArray(),
                values => values.Select(value => Enum.Parse<Amenity>(value)).ToList())
            .Metadata.SetValueComparer(AmenitiesComparer);

        builder.Property<uint>("Version").IsRowVersion();
    }

    private static string GetAllowedAmenities() =>
        string.Join(", ", Enum.GetNames<Amenity>().Select(amenity => $"'{amenity}'"));

    private static readonly ValueComparer<List<Amenity>> AmenitiesComparer = new(
        (left, right) => left!.SequenceEqual(right!),
        amenities => amenities.Aggregate(0, (hashCode, amenity) => HashCode.Combine(hashCode, amenity.GetHashCode())),
        amenities => amenities.ToList());
}
