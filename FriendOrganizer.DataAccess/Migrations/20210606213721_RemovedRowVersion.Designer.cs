﻿// <auto-generated />
using System;
using FriendOrganizer.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FriendOrganizer.DataAccess.Migrations
{
    [DbContext(typeof(FriendOrganizerDbContext))]
    [Migration("20210606213721_RemovedRowVersion")]
    partial class RemovedRowVersion
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.6");

            modelBuilder.Entity("FriendMeeting", b =>
                {
                    b.Property<int>("FriendsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MeetingsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("FriendsId", "MeetingsId");

                    b.HasIndex("MeetingsId");

                    b.ToTable("FriendMeeting (Dictionary<string, object>)");
                });

            modelBuilder.Entity("FriendOrganizer.Domain.Models.Friend", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int?>("FavoriteLanguageId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FavoriteLanguageId");

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

            modelBuilder.Entity("FriendOrganizer.Domain.Models.FriendPhoneNumber", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("FriendId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FriendId");

                    b.ToTable("FriendPhoneNumber");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            FriendId = 1,
                            Number = "+49-5361-9-0"
                        });
                });

            modelBuilder.Entity("FriendOrganizer.Domain.Models.Meeting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("DateFrom")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("DateTo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Meeting");

                    b.HasData(
                        new
                        {
                            Id = 2,
                            DateFrom = new DateTimeOffset(new DateTime(2021, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -4, 0, 0, 0)),
                            DateTo = new DateTimeOffset(new DateTime(2021, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -4, 0, 0, 0)),
                            Title = "Watching Football"
                        },
                        new
                        {
                            Id = 1,
                            DateFrom = new DateTimeOffset(new DateTime(2021, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -4, 0, 0, 0)),
                            DateTo = new DateTimeOffset(new DateTime(2021, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -4, 0, 0, 0)),
                            Title = "Watching Football"
                        });
                });

            modelBuilder.Entity("FriendOrganizer.Domain.Models.ProgrammingLanguage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ProgrammingLanguage");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "C#"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Python"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Swift"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Java"
                        },
                        new
                        {
                            Id = 5,
                            Name = "F#"
                        });
                });

            modelBuilder.Entity("FriendMeeting", b =>
                {
                    b.HasOne("FriendOrganizer.Domain.Models.Friend", null)
                        .WithMany()
                        .HasForeignKey("FriendsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FriendOrganizer.Domain.Models.Meeting", null)
                        .WithMany()
                        .HasForeignKey("MeetingsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FriendOrganizer.Domain.Models.Friend", b =>
                {
                    b.HasOne("FriendOrganizer.Domain.Models.ProgrammingLanguage", "FavoriteLanguage")
                        .WithMany()
                        .HasForeignKey("FavoriteLanguageId");

                    b.Navigation("FavoriteLanguage");
                });

            modelBuilder.Entity("FriendOrganizer.Domain.Models.FriendPhoneNumber", b =>
                {
                    b.HasOne("FriendOrganizer.Domain.Models.Friend", "Friend")
                        .WithMany("PhoneNumbers")
                        .HasForeignKey("FriendId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Friend");
                });

            modelBuilder.Entity("FriendOrganizer.Domain.Models.Friend", b =>
                {
                    b.Navigation("PhoneNumbers");
                });
#pragma warning restore 612, 618
        }
    }
}
