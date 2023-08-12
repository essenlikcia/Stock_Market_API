using System.ComponentModel.DataAnnotations;
using System.Configuration;
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
using Microsoft.AspNetCore.Authentication.Cookies;


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

            app.UseAuthentication();
            app.UseAuthorization();

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

                string jwtKey = "super_secret_key_for_JWT_Authentication_Admin";

                builder.Services
                    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    //auth
                    .AddCookie(opts =>
                    {
                        opts.Cookie.Name = $".web_app.auth";
                        opts.AccessDeniedPath = "/Home/AccessDenied";
                        opts.LoginPath = "/Home/Login";
                        opts.SlidingExpiration = true;
                    })
                    //jwt auth
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtKey)),
                            ClockSkew = TimeSpan.FromSeconds(5)
                        };
                    });
                builder.Services.AddSession(options =>
                {
                    options.Cookie.Name = $"web_app.session";
                    options.IdleTimeout = TimeSpan.FromMinutes(180);
                    options.Cookie.IsEssential = true;
                });
            }
        }
    }
}