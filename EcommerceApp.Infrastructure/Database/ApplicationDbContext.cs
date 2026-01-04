using EcommerceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<SellerOrder> SellerOrders { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<AdminProfile> AdminProfiles { get; set; }
        public DbSet<SellerProfile> SellerProfiles { get; set; }
        public DbSet<CustomerProfile> CustomerProfiles { get; set; }
        public DbSet<CustomerServiceProfile> CustomerServiceProfiles { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =====================================================
            // 1️. PROFILES → APPLICATION USER (1 : 1)
            // =====================================================

            //modelBuilder.Entity<AdminProfile>()
            //    .HasOne<ApplicationUser>()
            //    .WithOne()
            //    .HasForeignKey<AdminProfile>(x => x.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<AdminProfile>().HasKey(x => x.UserId);

            //modelBuilder.Entity<SellerProfile>()
            //    .HasOne<ApplicationUser>()
            //    .WithOne()
            //    .HasForeignKey<SellerProfile>(x => x.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<SellerProfile>().HasKey(x => x.UserId);
            //modelBuilder.Entity<SellerProfile>()
            //    .Property(x => x.Rating)
            //    .HasPrecision(18, 2);

            //modelBuilder.Entity<CustomerProfile>()
            //    .HasOne<ApplicationUser>()
            //    .WithOne()
            //    .HasForeignKey<CustomerProfile>(x => x.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<CustomerProfile>().HasKey(x => x.UserId);

            //modelBuilder.Entity<CustomerServiceProfile>()
            //    .HasOne<ApplicationUser>()
            //    .WithOne()
            //    .HasForeignKey<CustomerServiceProfile>(x => x.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<CustomerServiceProfile>().HasKey(x => x.UserId);

            //// 🔐 Ensure 1 profile per user
            //modelBuilder.Entity<AdminProfile>()
            //    .HasIndex(x => x.UserId).IsUnique();

            //modelBuilder.Entity<SellerProfile>()
            //    .HasIndex(x => x.UserId).IsUnique();

            //modelBuilder.Entity<CustomerProfile>()
            //    .HasIndex(x => x.UserId).IsUnique();

            //modelBuilder.Entity<CustomerServiceProfile>()
            //    .HasIndex(x => x.UserId).IsUnique();


            // AdminProfile
            modelBuilder.Entity<AdminProfile>()
                .HasOne(p => p.User)           // navigation property
                .WithOne()
                .HasForeignKey<AdminProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AdminProfile>().HasKey(p => p.UserId);

            // SellerProfile
            modelBuilder.Entity<SellerProfile>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<SellerProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SellerProfile>().HasKey(p => p.UserId);
            modelBuilder.Entity<SellerProfile>()
                .Property(x => x.Rating)
                .HasPrecision(18, 2);

            // CustomerProfile
            modelBuilder.Entity<CustomerProfile>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<CustomerProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CustomerProfile>().HasKey(p => p.UserId);

            // CustomerServiceProfile
            modelBuilder.Entity<CustomerServiceProfile>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<CustomerServiceProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CustomerServiceProfile>().HasKey(p => p.UserId);

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

            //modelBuilder.Entity<Order>()
            //    .Property(x => x.Discount)
            //    .HasPrecision(18, 2);

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
            // 4️. ORDER ↔ PRODUCT (M : M via SellerOrder)
            // =====================================================

            modelBuilder.Entity<SellerOrder>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<SellerOrder>()
                .HasOne(x => x.CorresponingOrder)
                .WithMany(o => o.SellerOrders)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SellerOrder>()
                .HasOne(x => x.OrderedProduct)
                .WithMany(p => p.BelongedSellerOrders)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SellerOrder>()
                .Property(o => o.Status)
                .HasConversion<string>() 
                .HasDefaultValue(OrderStatus.Pending); 

            //modelBuilder.Entity<SellerOrder>()
            //    .Property(x => x.UnitPrice)
            //    .HasPrecision(18, 2);

            // Prevent duplicate product in same order
            modelBuilder.Entity<SellerOrder>()
                .HasIndex(x => new { x.OrderId, x.ProductId })
                .IsUnique();
        }
    }
}
