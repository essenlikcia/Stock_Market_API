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
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Define the relationships between entities
            builder.Entity<Transaction>()
                .HasKey(t => new { t.Id, t.StockId });

            builder.Entity<Transaction>()
                .HasOne(t => t.Stock)
                .WithMany(s => s.Transactions)
                .HasForeignKey(t => t.StockId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Portfolio>()
                .HasKey(p => p.PortfolioId);

            // portfolio entity has a one-to-many relationship with the user entity
            // a user can have many portfolios,
            // but a portfolio can only belong to one user
            builder.Entity<Portfolio>()
                .HasOne(p => p.User)
                .WithMany(m => m.Portfolios)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

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
            var adminUserId = "admin-user-id";
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
                    PasswordHash = "21232f297a57a5a743894a0e4a801fc3",
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
            );
        }

    }
}
