using web_app.Data;

namespace web_app.Core.Repositories
{
    public interface IUnitOfWork
    {
        // Represents a repository for User-related data access operations.
        IUserRepository User { get; }

        // Represents a repository for Role-related data access operations.
        IRoleRepository Role { get; }

        // Represents a repository for Transaction-related data access operations.
        ITransactionRepository TransactionRepository { get; }

        // Represents a repository for Portfolio-related data access operations.
        IPortfolioRepository PortfolioRepository { get; }

        // Represents a repository for Stock-related data access operations.
        IStockRepository StockRepository { get; }

        // Asynchronously saves changes made in the context to the database.
        Task<int> SaveChangesAsync();
    }
}