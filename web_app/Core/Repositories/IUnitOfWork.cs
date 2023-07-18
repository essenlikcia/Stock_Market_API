namespace web_app.Core.Repositories;

public interface IUnitOfWork
{
    IUserRepository User { get; }

    IRoleRepository Role { get; }
}