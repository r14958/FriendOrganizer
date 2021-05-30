using FriendOrganizer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;

namespace FriendOrganizer.DataAccess
{
    public static class ModelBuilderExtensions
    {
        public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.DisplayName());
            }
        }

        public static void Seed(this ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Meeting>().HasData(
                new Meeting
                {
                    Id = 1,
                    Title = "Watching Football",
                    DateFrom = new DateTimeOffset(new DateTime(2021, 05, 29)),
                    DateTo = new DateTimeOffset(new DateTime(2021, 05, 29)),
                });

        }
    }
}

