using Microsoft.EntityFrameworkCore;
using web_app.Core.Repositories;
using web_app.Data;
using web_app.Models;

namespace web_app.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;
    // Constructor injection of ApplicationDbContext
    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Retrieves all transactions from the database.
    public async Task<IEnumerable<Transaction>> GetTransactionsAsync()
    {
        return await _context.Transactions.ToListAsync();
    }

    // Retrieves a specific transaction by its ID from the database.
    public async Task<Transaction> GetTransactionByIdAsync(string id)
    {
        return await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
    }

    // Adds a new transaction to the database.
    public async Task AddTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
    }

    // Updates an existing transaction in the database.
    public async Task UpdateTransactionAsync(Transaction transaction)
    {
        _context.Entry(transaction).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    // Deletes a specific transaction by its ID from the database.
    public async Task DeleteTransactionAsync(string id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction != null)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }
}