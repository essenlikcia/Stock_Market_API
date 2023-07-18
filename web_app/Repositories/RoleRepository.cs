using Microsoft.AspNetCore.Identity;
using web_app.Core.Repositories;
using web_app.Data;
using web_app.Models;

namespace web_app.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public ICollection<IdentityRole> GetRoles()
        {
            return _context.Roles.ToList();
        }
    }
}
