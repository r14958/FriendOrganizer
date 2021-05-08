using FriendOrganizer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;

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
            modelBuilder.Entity<Friend>().HasData(
               new Friend { Id = 1, FirstName = "Thomas", LastName = "Huber" },
               new Friend { Id = 2, FirstName = "Jeff", LastName = "Klein" },
               new Friend { Id = 3, FirstName = "Andreas", LastName = "Boehler" },
               new Friend { Id = 4, FirstName = "Chrissi", LastName = "Egin" }
               );
        }
    }
}

