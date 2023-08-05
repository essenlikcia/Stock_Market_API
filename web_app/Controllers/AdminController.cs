using Microsoft.AspNetCore.Mvc;
using web_app.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Differencing;
using Serilog;
using web_app.Core.Repositories;
using web_app.Core.ViewModel;
using web_app.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace web_app.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<CustomUser> _signInManager;
        //private readonly string _jwtSecretKey;

        public AdminController(IUnitOfWork unitOfWork, SignInManager<CustomUser> signInManager) //, string jwtSecretKey
        {
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
            //_jwtSecretKey = jwtSecretKey;
        }
        /*
        [HttpPost]
        public async Task<IActionResult> GenerateAdminJwtToken(string userId)
        {
            var user = _unitOfWork.User.GetUser(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Check if the user is an admin
            var isAdmin = await _signInManager.UserManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                return Unauthorized(); // Only admins can generate the token
            }

            // Generate the JWT token for admin user
            var jwtToken = JwtHelper.GenerateJwtToken(userId, "Admin", _jwtSecretKey, 60);

            // Return the token in the response, or use it as needed
            return Ok(new { token = jwtToken });
        }

        */
        public IActionResult Index()
        {
            var users = _unitOfWork.User.GetUsers();
            return View(users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = _unitOfWork.User.GetUser(id);
            var roles = _unitOfWork.Role.GetRoles();

            var userRoles = await _signInManager.UserManager.GetRolesAsync(user);

            /*
            var roleItems = new List<SelectListItem>();
            foreach (var role in roles)
            {
                var hasRole = userRoles.Any(ur => ur.Contains(role.Name));

                roleItems.Add(new SelectListItem(role.Name, role.Id, hasRole));
            }*/

            var roleItems = roles.Select(role =>
                new SelectListItem(
                    role.Name,
                    role.Id,
                    userRoles.Any(ur => ur.Contains(role.Name)))).ToList();

            var vm = new EditUserViewModel()
            {
                User = user,
                Roles = roleItems
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync(EditUserViewModel data)
        {
            var user = _unitOfWork.User.GetUser(data.User.Id);
            if (user == null)
            {
                return NotFound();
            }

            var userRolesInDb = await _signInManager.UserManager.GetRolesAsync(user);

            // loop through all the roles in viewmodel
            // check if the role is assigned in the database
            // if it is assigned, do nothing
            // if it is not assigned, add it to the user

            var rolesToAdd = new List<string>();
            var rolesToDelete = new List<string>();

            foreach (var role in data.Roles)
            {
                var assignedInDb = userRolesInDb.FirstOrDefault(ur => ur == role.Text);
                if (role.Selected)
                {
                    if (assignedInDb == null)
                    {
                        rolesToAdd.Add(role.Text);
                        // add role
                        await _signInManager.UserManager.AddToRoleAsync(user, role.Text);
                    }
                }
                else
                {
                    if (assignedInDb != null)
                    {
                        rolesToDelete.Add(role.Text);
                        // remove role
                        await _signInManager.UserManager.RemoveFromRoleAsync(user, role.Text);
                    }
                }
            }

            if (rolesToAdd.Any())
            {
                await _signInManager.UserManager.AddToRolesAsync(user, rolesToAdd);
            }

            if (rolesToDelete.Any())
            {
                await _signInManager.UserManager.RemoveFromRolesAsync(user, rolesToDelete);
            }

            user.FullName = data.User.FullName;
            user.Email = data.User.Email;

            _unitOfWork.User.UpdateUser(user);
            return RedirectToAction("Edit", new { id = user.Id });
        }


        /* Admin Setting Balance for user and system */
        [HttpPost]
        public async Task<IActionResult> SetBalance([FromBody]string userId, decimal newBalance)
        {
            var user = _unitOfWork.User.GetUser(userId);
            if (user == null)
            {
                return NotFound();
            }

            var portfolio = await _unitOfWork.PortfolioRepository.GetPortfolioByIdAsync(userId);
            if (portfolio == null)
            {
                return NotFound("Portfolio not found for the user."); 
            }

            portfolio.CurrentBalance = newBalance;
            await _unitOfWork.PortfolioRepository.UpdatePortfolioAsync(portfolio);
            await _unitOfWork.SaveChangesAsync();

            return Ok("Balance updated successfully.");
        }
        /* JSON Request Body
        {
            "userId": "user-id",
            "newBalance": 1500.00
        }*/
    }
}
