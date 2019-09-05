using RealEstateCore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace RealEstateCore.Infrastructure.DataContext
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<RealEstateProperty> RealEstateProperties { get; set; }
        public DbSet<PropertySettings> Settings { get; set; }
        public DbSet<Renter> Renter { get; set; }
        public DbSet<TransactionHistory> Transactions { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomTypes> RoomTypes { get; set; }
        public DbSet<RoomFloorPlan> FloorPlans { get; set; }
        public DbSet<RoomFeatures> RoomFeatures { get; set; }
        public DbSet<RoomRented> RoomsRented { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region User Configuration
            builder
                .Entity<ApplicationUser>()
                .ToTable("Users")
                .HasKey(p => p.Id);

            builder
                .Entity<ApplicationUser>()
                .Property(p => p.FirstName)
                .IsRequired();

            builder
                .Entity<ApplicationUser>()
                .Property(p => p.LastName)
                .IsRequired();

            builder
                .Entity<ApplicationUser>()
                .Property(p => p.Email)
                .IsRequired();

            builder
                .Entity<ApplicationUser>()
                .Property(p => p.Password)
                .IsRequired();

            builder
                .Entity<ApplicationUser>()
                .Property(p => p.DateRegistered)
                .IsRequired();

            builder
                .Entity<ApplicationUser>()
                .Property(p => p.EmailConfirmed)
                .IsRequired();

            builder
                .Entity<ApplicationUser>()
                .Property(p => p.ClientId)
                .IsRequired();

            builder
                .Entity<ApplicationUser>()
                .Property(p => p.Salt)
                .IsRequired();
            #endregion

            #region Property Configuration
            builder
                .Entity<RealEstateProperty>()
                .ToTable("RealEstateProperties")
                .HasKey(p => p.Id);

            builder
                .Entity<RealEstateProperty>()
                .HasOne(u => u.User)
                .WithMany(p => p.Properties)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Entity<RealEstateProperty>()
                .Property(p => p.Name)
                .IsRequired();

            builder
                .Entity<RealEstateProperty>()
                .Property(p => p.Address)
                .IsRequired();

            builder
                .Entity<RealEstateProperty>()
                .Property(p => p.City)
                .IsRequired();

            builder
                .Entity<RealEstateProperty>()
                .Property(p => p.ContactNo)
                .IsRequired();

            builder
                .Entity<RealEstateProperty>()
                .Property(p => p.Owner)
                .IsRequired();

            builder
                .Entity<RealEstateProperty>()
                .Property(p => p.TotalRooms)
                .IsRequired();
            #endregion

            #region Renter Configuration
            builder
                .Entity<Renter>()
                .ToTable("Renters")
                .HasKey(p => p.Id);

            builder
                .Entity<Renter>()
                .HasOne(p => p.Property)
                .WithMany(r => r.Renters)
                .HasForeignKey(p => p.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Entity<Renter>()
                .Property(p => p.Name)
                .IsRequired();

            builder
                .Entity<Renter>()
                .Property(p => p.ContactNo)
                .IsRequired();

            builder
                .Entity<Renter>()
                .Property(p => p.Address)
                .IsRequired();

            builder
                .Entity<Renter>()
                .Property(p => p.Profession)
                .IsRequired();

            builder
                .Entity<Renter>()
                .Property(p => p.CheckIn)
                .IsRequired();

            builder
                .Entity<Renter>()
                .Property(p => p.CheckOut)
                .IsRequired();
            #endregion

            #region Property Settings Configuration
            builder
                .Entity<PropertySettings>()
                .ToTable("Settings")
                .HasKey(p => p.Id);

            builder
                .Entity<PropertySettings>()
                .HasOne(p => p.Property)
                .WithOne(s => s.Settings)
                .HasForeignKey<PropertySettings>(p => p.PropertyId);

            builder
                .Entity<PropertySettings>()
                .Property(p => p.MonthDeposit);

            builder
                .Entity<PropertySettings>()
                .Property(p => p.MonthAdvance);
            #endregion

            #region Transaction History Configurataion
            builder
                .Entity<TransactionHistory>()
                .ToTable("Transactions")
                .HasKey(p => p.Id);

            builder
                .Entity<TransactionHistory>()
                .HasOne(r => r.Renter)
                .WithMany(p => p.Transactions)
                .HasForeignKey(r => r.RenterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Entity<TransactionHistory>()
                .Property(p => p.DatePaid)
                .IsRequired();

            builder
                .Entity<TransactionHistory>()
                .Property(p => p.AmountDue)
                .IsRequired();

            builder
                .Entity<TransactionHistory>()
                .Property(p => p.AmountPaid)
                .IsRequired();

            builder
                .Entity<TransactionHistory>()
                .Property(p => p.PaymentFor)
                .IsRequired();

            builder
                .Entity<TransactionHistory>()
                .Property(p => p.Balance)
                .IsRequired();

            builder
                .Entity<TransactionHistory>()
                .Property(p => p.NextDateDue)
                .IsRequired();

            builder
                .Entity<TransactionHistory>()
                .Property(p => p.DaysBeforeDue)
                .IsRequired();
            #endregion

            #region Rooms Configuration
            builder
                .Entity<Room>()
                .ToTable("Rooms")
                .HasKey(p => p.Id);

            builder
                .Entity<Room>()
                .HasOne(p => p.Property)
                .WithMany(r => r.Rooms)
                .HasForeignKey(p => p.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Entity<Room>()
                .Property(p => p.Name)
                .IsRequired();

            builder
                .Entity<Room>()
                .Property(p => p.TotalBeds)
                .IsRequired();

            builder
                .Entity<Room>()
                .Property(p => p.RoomTypeId)
                .IsRequired();
            #endregion

            #region Room Types Configuration
            builder
                .Entity<RoomTypes>()
                .ToTable("RoomTypes")
                .HasKey(p => p.Id);

            builder
                .Entity<RoomTypes>()
                .HasOne(p => p.Property)
                .WithMany(r => r.RoomTypes)
                .HasForeignKey(p => p.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Entity<RoomTypes>()
                .Property(p => p.Type)
                .IsRequired();

            builder
                .Entity<RoomTypes>()
                .Property(p => p.Price)
                .IsRequired();
            #endregion

            #region Floor Plans Configuration
            builder
                .Entity<RoomFloorPlan>()
                .ToTable("FloorPlans")
                .HasKey(p => p.Id);

            builder
                .Entity<RoomFloorPlan>()
                .HasOne(r => r.Room)
                .WithMany(r => r.FloorPlans)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Entity<RoomFloorPlan>()
                .Property(p => p.Img)
                .IsRequired(false);
            #endregion

            #region Room Features Configuration
            builder
                .Entity<RoomFeatures>()
                .ToTable("RoomFeatures")
                .HasKey(p => p.Id);

            builder
                .Entity<RoomFeatures>()
                .HasOne(r => r.Room)
                .WithMany(r => r.Features)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Entity<RoomFeatures>()
                .Property(p => p.Name)
                .IsRequired();
            #endregion

            #region Rooms Rented Configuration
            builder
                .Entity<RoomRented>()
                .HasKey(rr => new { rr.RoomId, rr.RenterId });

            builder
                .Entity<RoomRented>()
                .HasOne(r => r.Room)
                .WithMany(rr => rr.RoomsRented)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder
                .Entity<RoomRented>()
                .HasOne(r => r.Renter)
                .WithMany(rr => rr.RoomsRented)
                .HasForeignKey(r => r.RenterId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            #endregion
        }
    }
}
