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

            builder.HasData(
               new Friend { Id = 1, FirstName = "Thomas", LastName = "Huber" },
               new Friend { Id = 2, FirstName = "Jeff", LastName = "Klein" },
               new Friend { Id = 3, FirstName = "Andreas", LastName = "Boehler" },
               new Friend { Id = 4, FirstName = "Chrissi", LastName = "Egin" }
               );

        }
    }
}
