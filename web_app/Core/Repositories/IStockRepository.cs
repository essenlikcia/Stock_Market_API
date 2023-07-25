using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using web_app.Models;

namespace web_app.Data
{
    public interface IStockRepository
    {
        Task<IEnumerable<Stock>> GetStocksAsync();
        Task<Stock> GetStockByIdAsync(string id);
        Task<Stock> GetStockBySymbolAsync(string symbol);
        Task AddStockAsync(Stock stock);
        Task UpdateStockAsync(Stock stock);
        Task DeleteStockByIdAsync(string id);
    }
}