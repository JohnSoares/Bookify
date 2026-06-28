using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.FirstName)
            .HasMaxLength(FirstName.MaxLength)
            .HasConversion(
                firstName => firstName.Value,
                value => FirstName.Create(value).Value);

        builder.Property(user => user.LastName)
            .HasMaxLength(LastName.MaxLength)
            .HasConversion(
                lastName => lastName.Value,
                value => LastName.Create(value).Value);

        builder.Property(user => user.Email)
            .HasMaxLength(400)
            .HasConversion(email => email.Value, value => Email.Create(value).Value);

        builder.HasIndex(user => user.Email).IsUnique();

        builder.HasIndex(user => user.IdentityId).IsUnique();
    }
}
