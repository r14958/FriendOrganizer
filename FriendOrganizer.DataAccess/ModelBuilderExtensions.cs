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

            modelBuilder.Entity<ProgrammingLanguage>().HasData(
                new ProgrammingLanguage { Id = 1, Name = "C#" },
                new ProgrammingLanguage { Id = 2, Name = "Python" },
                new ProgrammingLanguage { Id = 3, Name = "Swift" },
                new ProgrammingLanguage { Id = 4, Name = "Java" },
                new ProgrammingLanguage { Id = 5, Name = "F#" });

            modelBuilder.Entity<FriendPhoneNumber>().HasData(
                new FriendPhoneNumber { Id = 1, Number = "+49 12345678", FriendId = 1 });
        }
    }
}

