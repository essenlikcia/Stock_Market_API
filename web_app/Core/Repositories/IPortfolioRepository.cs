using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using web_app.Models;

namespace web_app.Data
{
    public interface IPortfolioRepository
    {
        Task<IEnumerable<Portfolio>> GetPortfoliosAsync();
        Task<Portfolio> GetPortfolioByIdAsync(string id);
        Task AddPortfolioAsync(Portfolio portfolio);
        Task UpdatePortfolioAsync(Portfolio portfolio);
        Task DeletePortfolioAsync(string id);
    }
}