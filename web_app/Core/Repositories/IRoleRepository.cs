using Microsoft.AspNetCore.Identity;
using web_app.Models;

namespace web_app.Core.Repositories;

public interface IRoleRepository
{
    ICollection<IdentityRole> GetRoles();
}