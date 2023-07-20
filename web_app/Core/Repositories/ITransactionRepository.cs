using web_app.Models;

namespace web_app.Core.Repositories
{
    public interface ITransactionRepository
    {
        // Retrieves all transactions from the database asynchronously.
        Task<IEnumerable<Transaction>> GetTransactionsAsync();

        // Retrieves a specific transaction by its ID from the database asynchronously.
        Task<Transaction> GetTransactionByIdAsync(string id);

        // Adds a new transaction to the database asynchronously.
        Task AddTransactionAsync(Transaction transaction);

        // Updates an existing transaction in the database asynchronously.
        Task UpdateTransactionAsync(Transaction transaction);

        // Deletes a specific transaction by its ID from the database asynchronously.
        Task DeleteTransactionAsync(string id);
    }
}