using EcommerceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<SellerOrder> SellerOrders { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<AdminProfile> AdminProfiles { get; set; }
        public DbSet<SellerProfile> SellerProfiles { get; set; }
        public DbSet<CustomerProfile> CustomerProfiles { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =====================================================
            // 1️. PROFILES → APPLICATION USER (1 : 1)
            // =====================================================

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

            // ProductImages → Product
            modelBuilder.Entity<ProductImage>()
                .HasOne(x => x.CorrespondingProduct)
                .WithMany(o => o.ProductImages)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Feedback → Customer
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Customer)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Feedback → Seller
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Seller)
                .WithMany()
                .HasForeignKey(f => f.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Feedback → SellerOrder
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.CorrespondingSellerOrder)
                .WithMany()
                .HasForeignKey(f => f.SellerOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Prevent duplicate feedback for same order
            modelBuilder.Entity<Feedback>()
                .HasIndex(x => x.SellerOrderId)
                .IsUnique();

            // =====================================================
            // 5. CHATS AND MESSAGES TABLE
            // =====================================================
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.ToTable("Chats");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.SellerId)
                      .IsRequired();

                entity.Property(c => c.CustomerId)
                      .IsRequired();

                entity.Property(c => c.IsClosed)
                      .HasDefaultValue(false);

                entity.Property(c => c.CreatedAt)
                      .IsRequired();

                entity.Property(c => c.LastMessagedAt)
                      .IsRequired();

                // One Seller → Many Chats
                entity.HasOne(c => c.Seller)
                      .WithMany(a => a.SellerChats)
                      .HasForeignKey(c => c.SellerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // One Customer → Many Chats
                entity.HasOne(c => c.Customer)
                      .WithMany(a => a.CustomerChats)
                      .HasForeignKey(c => c.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // One Chat → Many Messages
                entity.HasMany(c => c.Messages)
                      .WithOne(m => m.CorrespondingChat)
                      .HasForeignKey(m => m.ChatId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Prevent duplicate chats between same seller & customer
                entity.HasIndex(c => new { c.SellerId, c.CustomerId })
                      .IsUnique();
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Messages");

                entity.HasKey(m => m.Id);

                entity.Property(m => m.ChatId)
                      .IsRequired();

                entity.Property(m => m.SenderId)
                      .IsRequired();

                entity.Property(m => m.SenderRole)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(m => m.Content)
                      .IsRequired()
                      .HasMaxLength(2000);

                entity.Property(m => m.IsRead)
                      .HasDefaultValue(false);

                entity.Property(m => m.MessagedAt)
                      .IsRequired();

                // One Sender → Many Messages
                entity.HasOne(m => m.Sender)
                      .WithMany(a => a.Messages)
                      .HasForeignKey(m => m.SenderId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Indexes for performance
                entity.HasIndex(m => m.ChatId);
                entity.HasIndex(m => new { m.ChatId, m.MessagedAt });

                // enforce allowed roles at DB level (SQL Server)
                entity.ToTable( t => t.HasCheckConstraint (
                    "CK_Message_SenderRole",
                    "[SenderRole] IN ('Customer', 'Seller')"
                ));
            });
        }
    }
}
