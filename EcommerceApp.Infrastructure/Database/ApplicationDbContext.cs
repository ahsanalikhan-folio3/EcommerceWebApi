using EcommerceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<AdminProfile> AdminProfiles { get; set; }
        public DbSet<SellerProfile> SellerProfiles { get; set; }
        public DbSet<CustomerProfile> CustomerProfiles { get; set; }
        public DbSet<CustomerServiceProfile> CustomerServiceProfiles { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =====================================================
            // 1️. PROFILES → APPLICATION USER (1 : 1)
            // =====================================================

            modelBuilder.Entity<AdminProfile>()
                .HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<AdminProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AdminProfile>().HasKey(x => x.UserId);

            modelBuilder.Entity<SellerProfile>()
                .HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<SellerProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SellerProfile>().HasKey(x => x.UserId);
            modelBuilder.Entity<SellerProfile>()
                .Property(x => x.Rating)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CustomerProfile>()
                .HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<CustomerProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CustomerProfile>().HasKey(x => x.UserId);

            modelBuilder.Entity<CustomerServiceProfile>()
                .HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<CustomerServiceProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CustomerServiceProfile>().HasKey(x => x.UserId);

            // 🔐 Ensure 1 profile per user
            modelBuilder.Entity<AdminProfile>()
                .HasIndex(x => x.UserId).IsUnique();

            modelBuilder.Entity<SellerProfile>()
                .HasIndex(x => x.UserId).IsUnique();

            modelBuilder.Entity<CustomerProfile>()
                .HasIndex(x => x.UserId).IsUnique();

            modelBuilder.Entity<CustomerServiceProfile>()
                .HasIndex(x => x.UserId).IsUnique();

            // =====================================================
            // 2️. CUSTOMER → ORDERS (1 : MANY)
            // =====================================================

            modelBuilder.Entity<Order>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .Property(x => x.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            modelBuilder.Entity<Order>()
                .Property(x => x.Status)
                .HasDefaultValue("Pending");

            // =====================================================
            // 3️. SELLER → PRODUCTS (1 : MANY)
            // =====================================================

            modelBuilder.Entity<Product>()
                .HasOne<SellerProfile>()
                .WithMany()
                .HasForeignKey(x => x.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);

            // =====================================================
            // 4️. ORDER ↔ PRODUCT (M : M via OrderItem)
            // =====================================================

            modelBuilder.Entity<OrderItem>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<OrderItem>()
                .HasOne(x => x.CorresponingOrder)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(x => x.OrderedProduct)
                .WithMany(p => p.BelongedOrderItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<OrderItem>()
            //    .Property(x => x.UnitPrice)
            //    .HasPrecision(18, 2);
            modelBuilder.Entity<OrderItem>()
                .Property(x => x.Discount)
                .HasPrecision(18, 2);

            // Prevent duplicate product in same order
            modelBuilder.Entity<OrderItem>()
                .HasIndex(x => new { x.OrderId, x.ProductId })
                .IsUnique();
        }
    }
}
