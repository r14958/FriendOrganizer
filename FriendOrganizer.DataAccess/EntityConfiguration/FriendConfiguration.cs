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
            builder.Property(f => f.FirstName).IsRequired();
            builder.Property(f => f.FirstName).HasMaxLength(50); // Ignored by SQLite

            builder.Property(f => f.LastName).IsRequired();
            builder.Property(f => f.LastName).HasMaxLength(50); // Ignored by SQLite

            builder.Property(f => f.Email).HasMaxLength(100); // Ignored by SQLite

        }
    }
}
