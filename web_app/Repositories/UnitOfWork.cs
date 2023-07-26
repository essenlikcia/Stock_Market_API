// UnitOfWork.cs
using Microsoft.EntityFrameworkCore;
using web_app.Core.Repositories;
using web_app.Data;
using System.Threading.Tasks;

namespace web_app.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IUserRepository User { get; }
        public IRoleRepository Role { get; }
        public ITransactionRepository TransactionRepository { get; private set; }
        public IPortfolioRepository PortfolioRepository { get; private set; }
        public IStockRepository StockRepository { get; private set; }
        public IStockHistoryRepository StockHistoryRepository { get; private set; }

        private readonly ApplicationDbContext _context;

        // Constructor for UnitOfWork with ApplicationDbContext and concrete repositories.
        public UnitOfWork(ApplicationDbContext context, IUserRepository userRepository, IRoleRepository roleRepository,
            ITransactionRepository transactionRepository, IPortfolioRepository portfolioRepository, IStockRepository stockRepository, IStockHistoryRepository stockHistoryRepository)
        {
            _context = context;
            User = userRepository;
            Role = roleRepository;
            TransactionRepository = transactionRepository;
            PortfolioRepository = portfolioRepository;
            StockRepository = stockRepository;
            StockHistoryRepository = stockHistoryRepository;
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
