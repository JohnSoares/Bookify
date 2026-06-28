using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.ToTable("apartments");

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

        builder.Property<uint>("Version").IsRowVersion();
    }
}
