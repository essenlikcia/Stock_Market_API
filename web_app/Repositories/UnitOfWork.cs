using Microsoft.EntityFrameworkCore;
using web_app.Core.Repositories;
using web_app.Data;
using System.Threading.Tasks;

namespace web_app.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        // Represents a repository for User-related data access operations.
        public IUserRepository User { get; }

        // Represents a repository for Role-related data access operations.
        public IRoleRepository Role { get; }

        // DbContext instance for database access.
        private readonly ApplicationDbContext _context;

        // Represents a repository for Transaction-related data access operations.
        public ITransactionRepository TransactionRepository { get; private set; }

        // Represents a repository for Portfolio-related data access operations.
        public IPortfolioRepository PortfolioRepository { get; private set; }

        // Represents a repository for Stock-related data access operations.
        public IStockRepository StockRepository { get; private set; }

        // Named constructor for dependency injection with ApplicationDbContext only.
        public static UnitOfWork CreateWithDbContext(ApplicationDbContext context)
        {
            return new UnitOfWork(context);
        }

        // Named constructor for dependency injection with additional repositories.
        public static UnitOfWork CreateWithRepositories(ApplicationDbContext context, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            return new UnitOfWork(context, userRepository, roleRepository);
        }

        // Constructor for UnitOfWork with ApplicationDbContext only.
        private UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            TransactionRepository = new TransactionRepository(_context);
            PortfolioRepository = new PortfolioRepository(_context);
            StockRepository = new StockRepository(_context);
        }

        // Constructor for UnitOfWork with _context and additional repositories.
        private UnitOfWork(ApplicationDbContext context, IUserRepository userRepository, IRoleRepository roleRepository)
            : this(context)
        {
            User = userRepository;
            Role = roleRepository;
        }

        // Asynchronously saves changes made in the context to the database.
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Disposes the _context when necessary.
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
