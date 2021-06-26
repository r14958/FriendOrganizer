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
    //public class AddressConfiguration : IEntityTypeConfiguration<Address>
    //{
        //public void Configure(EntityTypeBuilder<Address> builder)
        //{
        //    builder.ToTable(builder.Metadata.ClrType.Name);
        //    builder.Property(a => a.City).HasMaxLength(50);
        //    builder.Property(a => a.Street).HasMaxLength(50);
        //    builder.Property(a => a.StreetNumber).HasMaxLength(20);

        //    // Define Version (in EntityBase) as a RowVersion to enable concurrency.
        //    builder.Property(a => a.Version).HasDefaultValue(0).IsRowVersion();

            //builder.HasData(
            //    new Address { Id = 1, City = "Müllheim", Street = "Elmstreet", StreetNumber = "12345", FriendId = 1 },
            //    new Address { Id = 2, City = "Boxford", Street = "King John Drive", StreetNumber = "6", FriendId = 2 },
            //    new Address { Id = 3, City = "Tiengen", Street = "Hardstreet", StreetNumber = "5", FriendId = 3 },
            //    new Address { Id = 4, City = "Neuenburg", Street = "Rheinweg", StreetNumber = "4", FriendId = 4 }
            //    );
        //}
    //}
}
