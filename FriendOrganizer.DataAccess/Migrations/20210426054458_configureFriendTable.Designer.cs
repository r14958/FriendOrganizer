﻿// <auto-generated />
using FriendOrganizer.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FriendOrganizer.DataAccess.Migrations
{
    [DbContext(typeof(FriendOrganizerDbContext))]
    [Migration("20210426054458_configureFriendTable")]
    partial class ConfigureFriendTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.5");

            modelBuilder.Entity("FriendOrganizer.Model.Friend", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Friend");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            FirstName = "Thomas",
                            LastName = "Huber"
                        },
                        new
                        {
                            Id = 2,
                            FirstName = "Jeff",
                            LastName = "Klein"
                        },
                        new
                        {
                            Id = 3,
                            FirstName = "Andreas",
                            LastName = "Boehler"
                        },
                        new
                        {
                            Id = 4,
                            FirstName = "Chrissi",
                            LastName = "Egin"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
