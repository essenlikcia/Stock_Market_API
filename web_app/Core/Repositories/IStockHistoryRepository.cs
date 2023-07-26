using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using web_app.Models;

namespace web_app.Data
{
    public interface IStockHistoryRepository
    {
        Task<IEnumerable<StockHistory>> GetStocksHistoryAsync();
        Task<StockHistory> GetStockHistoryByIdAsync(string id);
        Task<StockHistory> GetStockHistoryBySymbolAsync(string symbol);
        Task AddStockHistoryAsync(StockHistory stock);
        Task UpdateStockHistoryAsync(StockHistory stock);
        Task DeleteStockHistoryByIdAsync(string id);
        Task AddStockHistoriesAsync(List<StockHistory> stockHistoryList);
    }
}