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
            builder.Property(pn => pn.Number).IsRequired();
        }
    }
}
