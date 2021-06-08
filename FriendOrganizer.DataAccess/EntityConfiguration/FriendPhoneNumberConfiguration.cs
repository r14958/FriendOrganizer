using FriendOrganizer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.DataAccess.EntityConfiguration
{
    public class FriendPhoneNumberConfiguration : IEntityTypeConfiguration<FriendPhoneNumber>
    {
        public void Configure(EntityTypeBuilder<FriendPhoneNumber> builder)
        {
            builder.ToTable(builder.Metadata.ClrType.Name);
            builder.Property(pn => pn.Number).IsRequired();

            // Define Version (in EntityBase) as a RowVersion to enable concurrency.
            builder.Property(pn => pn.Version).HasDefaultValue(0).IsRowVersion();

            builder.HasData(
                new FriendPhoneNumber { Id = 1, Number = "+49-5361-9-0", FriendId = 1 });
        }
    }
}
