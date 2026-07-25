namespace Events.Infrastructure.Persistence.Configurations;

using Events.Domain.Entities;
using Events.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => EventId.Of(value));

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.OrganizerId)
            .HasConversion(
                id => id.Value,
                value => UserId.Of(value));

        builder.Property(x => x.Status)
            .HasConversion<string>();

        builder.Property(x => x.StartsAt);

        builder.Property(x => x.EndsAt);

        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(x => x.Street).HasMaxLength(200);
            address.Property(x => x.Number).HasMaxLength(20);
            address.Property(x => x.District).HasMaxLength(100);
            address.Property(x => x.City).HasMaxLength(100);
            address.Property(x => x.State).HasMaxLength(100);
            address.Property(x => x.Country).HasMaxLength(100);
            address.Property(x => x.ZipCode).HasMaxLength(20);
        });
    }
}
