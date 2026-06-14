using DoorsWeb.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Metadata;

namespace DoorsWeb.API.Data
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private string? _SuperGuid;


        public DataContext(DbContextOptions<DataContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Retrieve the secret using the key
            _SuperGuid = _configuration["SuperGuid"] ??
                throw new InvalidOperationException("string 'SuperGuid' not found.");
        }

        public DbSet<AccessCalendar> AccessCalendar { get; set; }
        public DbSet<AccessLevel> AccessLevel { get; set; }
        public DbSet<AccessTimeZone> AccessTimeZone { get; set; }
        public DbSet<Admin> Admin { get; set; } 
        public DbSet<DoorHeader> DoorHeader { get; set; }
        public DbSet<DoorSettings> DoorSettings { get; set; }
        public DbSet<DoorStatus> DoorStatus { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<User> User { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Use .HasMany() first so that we don't have duplicates from other side
            // Except Calendar
            modelBuilder.Entity<AccessLevel>()
                .HasMany(e => e.Users)
                .WithMany(e => e.AccessLevels);
            modelBuilder.Entity<AccessLevel>()
                .HasMany(e => e.Doors)
                .WithMany(e => e.AccessLevels)
                .UsingEntity<AccessLevelElement>();
            modelBuilder.Entity<AccessLevel>()
                .HasIndex(u => u.Name)
                .IsUnique();
            modelBuilder.Entity<AccessLevel>()
                .Property(p => p.LastUpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // One-to-many without navigation to pricipal and optional shadow foreign key 
            modelBuilder.Entity<AccessTimeZone>()
                .HasMany(e => e.Elements)
                .WithOne()
                .IsRequired(false);
            // One-to-many without navigation to dependant and optional shadow foreign key
            modelBuilder.Entity<AccessTimeZone>()
                .HasOne(e => e.Calendar)
                .WithMany()
                .HasForeignKey(e => e.CalendarId)
                .IsRequired(false);
            modelBuilder.Entity<AccessTimeZone>()
                .HasIndex(u => u.Name)
                .IsUnique();
            modelBuilder.Entity<AccessTimeZone>()
                .Property(p => p.LastUpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // One-to-many without navigation to dependents and optional shadow foreign key
            modelBuilder.Entity<AccessTimeZoneElement>()
                .HasOne(e => e.Calendar)
                .WithMany()
                .HasForeignKey(e => e.CalendarId)
                .IsRequired(false);

            // One-to-many without navigation to pricipal and optional shadow foreign key 
            modelBuilder.Entity<AccessCalendar>()
                .HasMany(e => e.Elements)
                .WithOne()
                .IsRequired(false);
            modelBuilder.Entity<AccessCalendar>()
                .Property(p => p.LastUpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Admin>()
                .HasIndex(u => u.Username)
                .IsUnique();
            modelBuilder.Entity<Admin>()
                .Property(p => p.LastUpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");


            // Many-to-Many Relation with AccessLevel already defined, skipping.....
            modelBuilder.Entity<DoorHeader>()
                .HasIndex(u => u.ControllerId)
                .IsUnique();
            modelBuilder.Entity<DoorHeader>()
                .Property(p => p.LastUpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<DoorSettings>()
                .HasOne(e => e.Header)
                .WithOne(e => e.Settings)
                .HasForeignKey<DoorSettings>(e => e.Id)
                .IsRequired();
            modelBuilder.Entity<DoorSettings>()
                .Property(p => p.LastUpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<DoorStatus>()
                .HasOne(e => e.Header)
                .WithOne(e => e.Status)
                .HasForeignKey<DoorStatus>(e => e.Id)
                .IsRequired();
            modelBuilder.Entity<DoorStatus>()
                .Property(p => p.LastUpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // One-to-many without navigation to pricipal and optional shadow foreign key 
            modelBuilder.Entity<Event>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired(false);
            // One-to-many without navigation to pricipal and optional shadow foreign key 
            modelBuilder.Entity<Event>()
                .HasOne(e => e.DoorHeader)
                .WithMany()
                .HasForeignKey(e => e.DoorId)
                .IsRequired(false);
            // One-to-many without navigation to dependents and optional shadow foreign key
            modelBuilder.Entity<Event>()
                .HasOne(e => e.EventType)
                .WithMany()
                .HasForeignKey(e => e.EventTypeId)
                .IsRequired();

            // Many-to-Many Relation with AccessLevel already defined, skipping.....
            modelBuilder.Entity<User>()
                .Property(p => p.LastUpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");



            // Following are initialized seed data
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = new Guid(_SuperGuid),
                    Username = "super",
                    // Precomputed BCrypt hash of "654321". Must be a constant — hashing at model-build
                    // time produces a new salt each build, making the seed non-deterministic.
                    PasswordHash = "$2a$11$KaHdki2KyqrcPv9inpMeDuisvjAklAdKJeRqJuam.xpCRgjl00CUS",
                    CanEditUsers = true,
                    CanEditDoors = true,
                    CanEditAdmins = true
                });

            // Decide to have single truth in here, no redundant enums or dictionary
            modelBuilder.Entity<EventType>().HasData(
                new EventType { Id = 0, Name = "Card OK" },
                new EventType { Id = 1, Name = "Invalid Card" },
                new EventType { Id = 2, Name = "Card and PIN OK" },
                new EventType { Id = 3, Name = "Invalid PIN" },
                new EventType { Id = 4, Name = "Card Otl" },
                new EventType { Id = 5, Name = "Card Expired" },
                new EventType { Id = 6, Name = "Unknown Card" },
                new EventType { Id = 7, Name = "Anti Pass Back Failure" },
                new EventType { Id = 8, Name = "Request to Exit" },
                new EventType { Id = 9, Name = "Door Stuck Open" },
                new EventType { Id = 10, Name = "Fire Alarm" },
                new EventType { Id = 11, Name = "Fire Reset" },
                new EventType { Id = 12, Name = "Fire Fault" },
                new EventType { Id = 13, Name = "Intruder" },
                new EventType { Id = 14, Name = "Intruder Reset" },
                new EventType { Id = 15, Name = "Intruder Fault" },
                new EventType { Id = 16, Name = "Door Forced" },
                new EventType { Id = 17, Name = "Door Release by PC" },
                new EventType { Id = 18, Name = "Door Pc Close" },
                new EventType { Id = 19, Name = "Door Pc Open" },
                new EventType { Id = 20, Name = "Tz Door Open" },
                new EventType { Id = 21, Name = "Tz Door Close" },
                new EventType { Id = 22, Name = "Powered Down" },
                new EventType { Id = 23, Name = "Powered Up" },
                new EventType { Id = 24, Name = "Door Pc Release B" },
                new EventType { Id = 25, Name = "Door Pc Close B" },
                new EventType { Id = 26, Name = "Door Pc Open B" },
                new EventType { Id = 27, Name = "Tz Door Open B" },
                new EventType { Id = 28, Name = "Tz Door Close B" },
                new EventType { Id = 29, Name = "Invalid Code" },
                new EventType { Id = 30, Name = "Code Ok" },
                new EventType { Id = 31, Name = "Premature Card" },
                new EventType { Id = 32, Name = "Supply Low Volts" },
                new EventType { Id = 33, Name = "Supply High Volts" },
                new EventType { Id = 34, Name = "Code Duress" },
                new EventType { Id = 35, Name = "Hacker" },
                new EventType { Id = 36, Name = "Menu Engineer" },
                new EventType { Id = 37, Name = "Menu User" },
                new EventType { Id = 38, Name = "Random Search" },
                new EventType { Id = 39, Name = "Reader Error" },
                new EventType { Id = 46, Name = "Card 4 Pin" },
                new EventType { Id = 47, Name = "Pin Timeout" },
                new EventType { Id = 48, Name = "Card 4 Code" },
                new EventType { Id = 49, Name = "Transaction Timeout" },
                new EventType { Id = 50, Name = "Card Code Ok" },
                new EventType { Id = 54, Name = "Interlocked" },
                new EventType { Id = 55, Name = "Bio Unknown" },
                new EventType { Id = 56, Name = "Bio Aborted" },
                new EventType { Id = 57, Name = "Bio Con Rx Tmp Err" },
                new EventType { Id = 58, Name = "Bio Invalid" },
                new EventType { Id = 59, Name = "Bio Timeout" },
                new EventType { Id = 60, Name = "Bio Duplicated" },
                new EventType { Id = 61, Name = "Bio Rdr Tx Tmp Err" },
                new EventType { Id = 62, Name = "Sequence Err" },
                new EventType { Id = 63, Name = "Seq Ok" },
                new EventType { Id = 64, Name = "No Bio Template" },
                new EventType { Id = 65, Name = "Bio Template List" },
                new EventType { Id = 66, Name = "Bio Rdr Rx Tmp Err" },
                new EventType { Id = 67, Name = "Bio Template Updated" },
                new EventType { Id = 68, Name = "Bio Template Deleted" },
                new EventType { Id = 69, Name = "Bio Duress" },
                new EventType { Id = 70, Name = "Multi Factor Over Ride Tz Open" },
                new EventType { Id = 71, Name = "Multi Factor Over Ride Tz Close" },
                new EventType { Id = 73, Name = "Telekey Low Battery" },
                new EventType { Id = 74, Name = "Call Button" },
                new EventType { Id = 200, Name = "Key Beep" },
                new EventType { Id = 201, Name = "Key Nokey" },
                new EventType { Id = 202, Name = "Starkey Beep" },
                new EventType { Id = 203, Name = "Bio Start Enrol" },
                new EventType { Id = 204, Name = "Admin Id Ok" }
            );

        }
    }
}
