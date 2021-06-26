using FriendOrganizer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FriendOrganizer.DataAccess.EntityConfiguration
{
    /// <summary>
    /// Configures attributes on the <see cref="Friend"/> entity.
    /// </summary>
    /// <remarks>Note that MaxLength attributes are ignored by SQLite!</remarks>
    public class FriendConfiguration : IEntityTypeConfiguration<Friend>
    {
       
        public void Configure(EntityTypeBuilder<Friend> builder)
        {
            builder.ToTable(builder.Metadata.ClrType.Name);
            builder.Property(f => f.FirstName).IsRequired();
            builder.Property(f => f.FirstName).HasMaxLength(50); // Ignored by SQLite

            builder.Property(f => f.LastName).IsRequired();
            builder.Property(f => f.LastName).HasMaxLength(50); // Ignored by SQLite

            builder.Property(f => f.Email).HasMaxLength(100); // Ignored by SQLite

            // Define Version (in EntityBase) as a RowVersion to enable concurrency.
            builder.Property(f => f.Version).HasDefaultValue(0).IsRowVersion();

            builder.Property(f => f.IsDeveloper).HasDefaultValue(false);

            
            builder.OwnsOne(f => f.Address, a =>
            {
                a.ToTable(builder.Metadata.ClrType.Name);
                a.Property(a => a.City).HasMaxLength(50);
                a.Property(a => a.Street).HasMaxLength(50);
                a.Property(a => a.StreetNumber).HasMaxLength(20);
                a.Property(a => a.Version).HasDefaultValue(0).IsRowVersion();

                a.HasData(
                new Address { FriendId = 1, Id = 1, City = "Müllheim", Street = "Elmstreet", StreetNumber = "12345" },
                new Address { FriendId = 2, Id = 2, City = "Boxford", Street = "King John Drive", StreetNumber = "6" },
                new Address { FriendId = 3, Id = 3, City = "Tiengen", Street = "Hardstreet", StreetNumber = "5" },
                new Address { FriendId = 4, Id = 4, City = "Neuenburg", Street = "Rheinweg", StreetNumber = "4" }
                );
            });

            builder.HasData(
               new Friend { Id = 1, FirstName = "Thomas", LastName = "Huber" },
               new Friend { Id = 2, FirstName = "Jeff", LastName = "Klein" },
               new Friend { Id = 3, FirstName = "Andreas", LastName = "Boehler" },
               new Friend { Id = 4, FirstName = "Chrissi", LastName = "Egin" }
               );
        }
    }
}
