using Microsoft.AspNetCore.Mvc;
using web_app.Core.Repositories;
using web_app.Models;

namespace web_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        // Constructor injection of IUnitOfWork
        public TransactionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /api/Transaction
        // Retrieves all transactions from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            var transactions = await _unitOfWork.TransactionRepository.GetTransactionsAsync();
            return Ok(transactions);
        }

        // GET: /api/Transaction/5
        // Retrieves a specific transaction by its ID from the database.
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(string id)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        // POST: /api/Transaction
        // Adds a new transaction to the database
        // AddTransaction is designed to return the added Transaction
        [HttpPost]
        public async Task<ActionResult<Transaction>> AddTransaction(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.TransactionRepository.AddTransactionAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }


        // PUT: /api/Transaction/5
        // Updates an existing transaction in the database.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(string id, Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return BadRequest();
            }

            await _unitOfWork.TransactionRepository.UpdateTransactionAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            // Returns a "204 No Content" status to indicate successful update without returning a response body.
            return NoContent();
        }

        // DELETE: /api/Transaction/5
        // Deletes a specific transaction by its ID from the database.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(string id)
        {
            await _unitOfWork.TransactionRepository.DeleteTransactionAsync(id);
            await _unitOfWork.SaveChangesAsync();

            // Returns a "204 No Content" status to indicate successful deletion without returning a response body.
            return NoContent();
        }
    }
}
