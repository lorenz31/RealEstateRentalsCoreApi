﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RealEstateCore.Infrastructure.DataContext;

namespace RealEstateCore.Infrastructure.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20190421170832_AddedDaysBeforeDueToTransactionHistoryModel")]
    partial class AddedDaysBeforeDueToTransactionHistoryModel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("RealEstateCore.Core.Models.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientId")
                        .IsRequired();

                    b.Property<DateTime>("DateRegistered");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<string>("Salt")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.PropertySettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("MonthAdvance");

                    b.Property<decimal>("MonthDeposit");

                    b.Property<Guid>("PropertyId");

                    b.HasKey("Id");

                    b.HasIndex("PropertyId")
                        .IsUnique();

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.RealEstateProperty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<string>("City")
                        .IsRequired();

                    b.Property<string>("ContactNo")
                        .IsRequired();

                    b.Property<decimal>("Latitude");

                    b.Property<decimal>("Longitude");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Owner")
                        .IsRequired();

                    b.Property<int>("TotalRooms");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RealEstateProperties");
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.Renter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<DateTime>("CheckIn");

                    b.Property<DateTime>("CheckOut");

                    b.Property<string>("ContactNo")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Profession")
                        .IsRequired();

                    b.Property<Guid>("PropertyId");

                    b.HasKey("Id");

                    b.HasIndex("PropertyId");

                    b.ToTable("Renters");
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.Room", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid>("PropertyId");

                    b.Property<Guid>("RoomTypeId");

                    b.Property<int>("TotalBeds");

                    b.HasKey("Id");

                    b.HasIndex("PropertyId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.RoomFeatures", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid>("RoomId");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.ToTable("RoomFeatures");
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.RoomFloorPlan", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Img");

                    b.Property<Guid>("RoomId");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.ToTable("FloorPlans");
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.RoomRented", b =>
                {
                    b.Property<Guid>("RoomId");

                    b.Property<Guid>("RenterId");

                    b.HasKey("RoomId", "RenterId");

                    b.HasIndex("RenterId");

                    b.ToTable("RoomsRented");
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.RoomTypes", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Price");

                    b.Property<Guid>("PropertyId");

                    b.Property<string>("Type")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("PropertyId");

                    b.ToTable("RoomTypes");
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.TransactionHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("AmountDue");

                    b.Property<decimal>("AmountPaid");

                    b.Property<decimal>("Balance");

                    b.Property<DateTime>("DatePaid");

                    b.Property<DateTime>("DaysBeforeDue");

                    b.Property<DateTime>("NextDateDue");

                    b.Property<string>("PaymentFor")
                        .IsRequired();

                    b.Property<Guid>("RenterId");

                    b.HasKey("Id");

                    b.HasIndex("RenterId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.PropertySettings", b =>
                {
                    b.HasOne("RealEstateCore.Core.Models.RealEstateProperty", "Property")
                        .WithOne("Settings")
                        .HasForeignKey("RealEstateCore.Core.Models.PropertySettings", "PropertyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.RealEstateProperty", b =>
                {
                    b.HasOne("RealEstateCore.Core.Models.ApplicationUser", "User")
                        .WithMany("Properties")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.Renter", b =>
                {
                    b.HasOne("RealEstateCore.Core.Models.RealEstateProperty", "Property")
                        .WithMany("Renters")
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.Room", b =>
                {
                    b.HasOne("RealEstateCore.Core.Models.RealEstateProperty", "Property")
                        .WithMany("Rooms")
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.RoomFeatures", b =>
                {
                    b.HasOne("RealEstateCore.Core.Models.Room", "Room")
                        .WithMany("Features")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.RoomFloorPlan", b =>
                {
                    b.HasOne("RealEstateCore.Core.Models.Room", "Room")
                        .WithMany("FloorPlans")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.RoomRented", b =>
                {
                    b.HasOne("RealEstateCore.Core.Models.Renter", "Renter")
                        .WithMany("RoomsRented")
                        .HasForeignKey("RenterId");

                    b.HasOne("RealEstateCore.Core.Models.Room", "Room")
                        .WithMany("RoomsRented")
                        .HasForeignKey("RoomId");
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.RoomTypes", b =>
                {
                    b.HasOne("RealEstateCore.Core.Models.RealEstateProperty", "Property")
                        .WithMany("RoomTypes")
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RealEstateCore.Core.Models.TransactionHistory", b =>
                {
                    b.HasOne("RealEstateCore.Core.Models.Renter", "Renter")
                        .WithMany("Transactions")
                        .HasForeignKey("RenterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
