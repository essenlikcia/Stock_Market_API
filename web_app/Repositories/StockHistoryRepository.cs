using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using web_app.Models;
namespace web_app.Data
{
    public class StockHistoryRepository : IStockHistoryRepository
    {
        private readonly ApplicationDbContext _context;
        // Constructor injection of ApplicationDbContext

        public StockHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockHistory>> GetStocksHistoryAsync()
        {
            return await _context.StockHistories.ToListAsync();
        }

        public async Task<StockHistory> GetStockHistoryByIdAsync(string id)
        {
            return await _context.StockHistories.FirstOrDefaultAsync(s => s.StockHistoryID.ToString() == id); ;
        }


        public async Task<StockHistory> GetStockHistoryBySymbolAsync(string symbol)
        {
            return await _context.StockHistories.FirstOrDefaultAsync(s => s.Symbol == symbol);
        }
        public async Task AddStockHistoryAsync(StockHistory stock)
        {
            _context.StockHistories.Add(stock);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStockHistoryAsync(StockHistory stock)
        {
            _context.Entry(stock).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStockHistoryByIdAsync(string id)
        {
            var stock = await _context.StockHistories.FirstOrDefaultAsync(s => s.StockHistoryID.ToString() == id);
            if (stock != null)
            {
                _context.StockHistories.Remove(stock);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddStockHistoriesAsync(List<StockHistory> stockHistoryList)
        {
            _context.StockHistories.AddRange(stockHistoryList);
            await _context.SaveChangesAsync();
        }
    }
}