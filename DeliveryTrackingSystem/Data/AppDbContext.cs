using DeliveryTrackingSystem.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DeliveryTrackingSystem.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentStatusHistory> ShipmentStatusHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تخصيص Sequence واحدة لجميع الـ IDs
            modelBuilder.HasSequence<int>("CommonSequence", schema: "dbo")
                .StartsAt(500)
                .IncrementsBy(6); // 6 الزيادة بمقدار 

            // Id start with 500 and increase with 6 for all Ids
            modelBuilder.Entity<User>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEXT VALUE FOR dbo.CommonSequence");

            modelBuilder.Entity<Customer>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEXT VALUE FOR dbo.CommonSequence");

            modelBuilder.Entity<Shipment>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEXT VALUE FOR dbo.CommonSequence");

            modelBuilder.Entity<ShipmentStatusHistory>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEXT VALUE FOR dbo.CommonSequence");


            // علاقة 1-1 بين ApplicationUser و User
            modelBuilder.Entity<User>()
                .HasOne(u => u.ApplicationUser)
                .WithOne()
                .HasForeignKey<User>(u => u.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // العلاقة بين Driver (User) و Shipments
            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Driver)
                .WithMany(d => d.AssignedShipments)
                .HasForeignKey(s => s.DriverId)
                .OnDelete(DeleteBehavior.SetNull);

            // علاقة 1-1 بين Shipment و Customer
            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.Shipments)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Roles
            var employeeId = "0c20a355-12dd-449d-a8d5-6e33960c45ee";
            var driverId = "393f1091-b175-4cca-a1df-19971e86e2a3";
            var adminId = "7d090697-295a-43bf-bb0b-3a19843fb528";
            var superAdminId = "8b2fbfe2-0a51-4f8e-b57f-4504d20a3739";

            var roles = new List<IdentityRole>
         {
             new IdentityRole
             {
                 Id = employeeId,
                 ConcurrencyStamp =employeeId,
                 Name = "Employee",
                 NormalizedName = "Employee".ToUpper()
             },
             new IdentityRole
             {
                 Id = driverId,
                 ConcurrencyStamp =driverId,
                 Name = "Driver",
                 NormalizedName = "Driver".ToUpper()
             },
             new IdentityRole
             {
                 Id = adminId,
                 ConcurrencyStamp =adminId,
                 Name = "Admin",
                 NormalizedName = "Admin".ToUpper()
             },
             new IdentityRole
             {
                 Id = superAdminId,
                 ConcurrencyStamp =superAdminId,
                 Name = "SuperAdmin",
                 NormalizedName = "SuperAdmin".ToUpper()
             }
                    };

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.UserName)
                      .HasColumnType("nvarchar(256)"); // استخدم NVARCHAR للأحرف العربية
            });

            modelBuilder.Entity<IdentityRole>().HasData(roles);

        }
    }
}
