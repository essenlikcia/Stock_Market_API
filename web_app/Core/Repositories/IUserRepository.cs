using web_app.Models;

namespace web_app.Core.Repositories
{
    public interface IUserRepository
    {
        ICollection<CustomUser> GetUsers();

        CustomUser GetUser(string id);

        CustomUser UpdateUser(CustomUser user);
    }
}
