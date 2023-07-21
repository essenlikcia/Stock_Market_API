using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using web_app.Models;

namespace web_app.Data
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;
        // Constructor injection of ApplicationDbContext

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Stock>> GetStocksAsync()
        {
            return await _context.Stocks.ToListAsync();
        }

        public async Task<Stock> GetStockByIdAsync(string id)
        {
            return await _context.Stocks.FindAsync(id);
        }

        public async Task AddStockAsync(Stock stock)
        {
            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStockAsync(Stock stock)
        {
            _context.Entry(stock).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStockAsync(string id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock != null)
            {
                _context.Stocks.Remove(stock);
                await _context.SaveChangesAsync();
            }
        }
    }
}