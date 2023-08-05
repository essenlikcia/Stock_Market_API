using Ecng.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using web_app.Models;

namespace web_app.Data
{
    public class ApplicationDbContext : IdentityDbContext<CustomUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        // seed stock, transaction and portfolio tables
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockHistory> StockHistories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Define the relationships between entities
            // transaction entity has composite primary key
            // one transaction, multiple stocks
            builder.Entity<Transaction>()
                .HasKey(t => new { t.Id, t.StockId });

            builder.Entity<Transaction>()
                .HasOne(t => t.Stock) // Transaction entity has one Stock
                .WithMany(s => s.Transactions) // Stock entity has many Transactions
                .HasForeignKey(t => t.StockId) // Transaction entity has foreign key StockId
                .OnDelete(DeleteBehavior.Restrict); // when a stock is deleted, its transactions are not deleted

            // transaction entity has a one-to-many relationship with the portfolio entity
            builder.Entity<Transaction>()
                .HasOne(t => t.User) // transaction entity has one user
                .WithMany(u => u.Transactions) // user entity has many transactions
                .HasForeignKey(t => t.Id) // transaction entity has foreign key Id
                .OnDelete(DeleteBehavior.Restrict); // when a user is deleted, their transactions are not deleted

            // portfolio entity has primary key PortfolioId
            builder.Entity<Portfolio>()
                .HasKey(p => p.PortfolioId);

            // portfolio entity has a one-to-many relationship with the user entity
            // a user can have many portfolios,
            // but a portfolio can only belong to one user
            builder.Entity<Portfolio>()
                .HasOne(p => p.User) // portfolio entity has one user
                .WithMany(m => m.Portfolios)// user entity has many portfolios
                .HasForeignKey(p => p.UserId) // portfolio entity has foreign key UserId
                .OnDelete(DeleteBehavior.Restrict); // when a user is deleted, their portfolios are not deleted
            
            builder.Entity<StockHistory>()
                .HasOne(sh => sh.Stock) // StockHistory entity has one Stock
                .WithMany(s => s.StockHistories) // Stock entity has many StockHistories
                .HasForeignKey(sh => sh.StockId) // StockHistory entity has foreign key StockId
                .OnDelete(DeleteBehavior.Cascade); // when a Stock is deleted, its StockHistories will also be deleted

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "1",
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = null
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = null
                });

            // Add first user as Admin if it's the only user in IdentityUser table
            /*var adminUserId = "admin-user-id";
            var adminRoleId = "2"; // Admin role Id
            builder.Entity<CustomUser>().HasData(
                new CustomUser
                {
                    Id = adminUserId,
                    UserName = "adminuser",
                    NormalizedUserName = "ADMINUSER",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = null,
                    SecurityStamp = null,
                    ConcurrencyStamp = null,
                    PhoneNumber = null,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnd = null,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    Address = "admin-address",
                    BirthDate = DateTime.Today,
                    FullName = "admin",
                    Gender = "male"
                }
            );

            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                }
            );*/
        }

    }
}
