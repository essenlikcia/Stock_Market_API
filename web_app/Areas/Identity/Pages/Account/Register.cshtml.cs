// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using web_app;
using web_app.Core.Repositories;
using web_app.Data;
using web_app.Models;

namespace web_app.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly UserManager<CustomUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<CustomUser> _userStore;
        private readonly IUserEmailStore<CustomUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterModel(
            UserManager<CustomUser> userManager,
            IUserStore<CustomUser> userStore,
            SignInManager<CustomUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, RoleManager<IdentityRole> roleManager,
            IPortfolioRepository portfolioRepository,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _portfolioRepository = portfolioRepository;
            _unitOfWork = unitOfWork;
        }
        [BindProperty]
        public bool IsAdmin { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public class InputModel
        {
            /*
            [Required]
            [Display(Name = "Register Type")]*/
            public string Role { get; set; }

            // username already exist but we added username because we dont want email to be username
            [Required]
            [Display(Name = "User Name")]
            public string UserName { get; set; }

            [Required]
            [Display(Name = "Name and Surname")]
            public string FullName { get; set; }
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required] [Display(Name = "Gender")] public string Gender { get; set; }

            [Required] [Display(Name = "Address")] public string Address { get; set; }

            [Required]
            [Display(Name = "Date of Birth")]
            public DateTime DateOfBirth { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(bool isAdmin, string returnUrl = null) // string role
        {
            returnUrl ??= Url.Content("~/");
            // _roleManager.CreateAsync(new IdentityRole("User Registration")).Wait();
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new CustomUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = Input.UserName,
                    Email = Input.Email,
                    FullName = Input.FullName,
                    Address = Input.Address,
                    Gender = Input.Gender,
                    BirthDate = Input.DateOfBirth,
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (!result.Succeeded)
                {
                    _logger.LogInformation("couldnt create :(");
                }
                else
                {
                    //var addRoleToUser = await _userManager.AddToRoleAsync(user, Input.Role);
                    if (result.Succeeded)
                    {
                        var portfolio = new Portfolio
                        {
                            UserId = user.Id,
                            Name = "Default Portfolio",
                            Description = "Default portfolio for the user",
                            InitialBalance = 10000, // Set the initial balance here if needed
                            CurrentBalance = 10000,
                            TotalProfit = 0,
                            TotalProfitPercentage = 0,
                            Quantity = 0
                        };

                        await _portfolioRepository.AddPortfolioAsync(portfolio);
                        // await _unitOfWork.SaveChangesAsync();

                        // added for configuring user in the first time to be admin and later on user
                        if (_userManager.Users.Count() == 1 )
                        {
                            await _userManager.AddToRoleAsync(user, "Admin");
                            await _userManager.AddToRoleAsync(user, "User");
                            /*bool isFirstUser = await _userManager.IsInRoleAsync(user, "Admin");
                            if (isFirstUser)
                            {
                                // Generate JWT token
                                var tokenHandler = new JwtSecurityTokenHandler();
                                var key = Encoding.ASCII.GetBytes("super_secret_key_for_JWT_Authentication_Admin"); // same key from program.cs
                                var tokenDescriptor = new SecurityTokenDescriptor
                                {
                                    Subject = new ClaimsIdentity(new Claim[]
                                    {
                                        new Claim(ClaimTypes.Name, user.UserName),
                                        new Claim(ClaimTypes.Role, "Admin")
                                    }),
                                    Expires = DateTime.UtcNow.AddDays(7), // Token expiration time
                                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                                };
                                var token = tokenHandler.CreateToken(tokenDescriptor);
                                var jwtToken = tokenHandler.WriteToken(token);

                                // Return JSONResult along with the JWT token
                                return new JsonResult(new
                                {
                                    Message = "Admin User created a new account with password.",
                                    Token = jwtToken
                                });
                            }*/
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, "User");
                        }

                        if (IsAdmin)
                        {
                            await _userManager.AddToRoleAsync(user, "Admin");
                            await _userManager.AddToRoleAsync(user, "User");
                        }

                        _logger.LogInformation("User created a new account with password.");
                        /*added*/
                        // string roleId = Input.Role;
                        // await _userManager.AddToRoleAsync(user, roleId);
                        /*added*/

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                        /*
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation",
                                new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }*/
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private CustomUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<CustomUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(CustomUser)}'. " +
                                                    $"Ensure that '{nameof(CustomUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                                                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<CustomUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<CustomUser>)_userStore;
        }
    }
}