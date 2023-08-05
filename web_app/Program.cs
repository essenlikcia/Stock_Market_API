using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serilog;
using web_app.Core.Repositories;
using web_app.Data;
using web_app.Models;
using web_app.Repositories;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.IdentityModel.Tokens;
using web_app.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace web_app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<CustomUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>() // added roles
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            // Add services to the container.
            AddScoped();

            // logger
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(new LoggerConfiguration()
                    .WriteTo.File("C:\\Users\\Xavier\\Desktop\\Logs\\API_Log.log",
                        rollingInterval: RollingInterval.Day)
                    .CreateLogger());
            });

            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // ef migrations
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();

            void AddScoped()
            {
                builder.Services.AddScoped<IUserRepository, UserRepository>(); // Register UserRepository as the implementation for IUserRepository
                builder.Services.AddScoped<IRoleRepository, RoleRepository>();
                builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
                builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
                builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
                builder.Services.AddScoped<IStockRepository, StockRepository>();
                builder.Services.AddScoped<IStockHistoryRepository, StockHistoryRepository>();
                builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
                /*var key = Encoding.ASCII.GetBytes("super_secret_key_for_JWT_Authentication_Admin"); // Replace with your actual secret key
                builder.Services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
                builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                });
                */
            }
        }
    }
}