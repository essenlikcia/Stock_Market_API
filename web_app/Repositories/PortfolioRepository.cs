using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using web_app.Models;

namespace web_app.Data
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDbContext _context;

        public PortfolioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Portfolio>> GetPortfoliosAsync()
        {
            return await _context.Portfolios.ToListAsync();
        }

        public async Task<Portfolio> GetPortfolioByIdAsync(string id)
        {
            return await _context.Portfolios.FindAsync(id);
        }

        public async Task AddPortfolioAsync(Portfolio portfolio)
        {
            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePortfolioAsync(Portfolio portfolio)
        {
            _context.Entry(portfolio).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeletePortfolioAsync(string id)
        {
            var portfolio = await _context.Portfolios.FindAsync(id);
            if (portfolio != null)
            {
                _context.Portfolios.Remove(portfolio);
                await _context.SaveChangesAsync();
            }
        }
    }
}