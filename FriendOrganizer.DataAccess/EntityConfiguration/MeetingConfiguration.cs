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
    public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
    {
        public void Configure(EntityTypeBuilder<Meeting> builder)
        {
            builder.ToTable(builder.Metadata.ClrType.Name);
            builder.Property(m => m.Title).IsRequired();
            builder.Property(m => m.Title).HasMaxLength(50);

            builder.HasData(
                new Meeting
                {
                    Id = 2,
                    Title = "Watching Football",
                    DateFrom = new DateTimeOffset(new DateTime(2021, 05, 29)),
                    DateTo = new DateTimeOffset(new DateTime(2021, 05, 29)),
                });
        }
    }
}
