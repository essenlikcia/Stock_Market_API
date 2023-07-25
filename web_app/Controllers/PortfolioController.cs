using Microsoft.AspNetCore.Mvc;
using web_app.Core.Repositories;
using web_app.Models;
using System.Threading.Tasks;

namespace web_app.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PortfolioController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Portfolio
        // Retrieves all portfolios from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Portfolio>>> GetPortfolios()
        {
            // Get all portfolios asynchronously from the repository through the Unit of Work.
            var portfolios = await _unitOfWork.PortfolioRepository.GetPortfoliosAsync();
            return Ok(portfolios);
        }

        // GET: api/Portfolio/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Portfolio>> GetPortfolio(string id)
        {
            // Get a specific portfolio by its ID asynchronously from the repository through the Unit of Work.
            var portfolio = await _unitOfWork.PortfolioRepository.GetPortfolioByIdAsync(id);
            if (portfolio == null)
            {
                return NotFound();
            }

            return portfolio;
        }
        
        // POST: api/Portfolio
        [HttpPost]
        public async Task<ActionResult<Portfolio>> AddPortfolio([FromBody]Portfolio portfolio)
        {
            if (!ModelState.IsValid)
            {
                // If the incoming data is invalid, return a BadRequest response with the ModelState errors.
                return BadRequest(ModelState);
            }

            // Add the new portfolio asynchronously to the repository through the Unit of Work.
            await _unitOfWork.PortfolioRepository.AddPortfolioAsync(portfolio);
            // Save the changes to the database asynchronously through the Unit of Work.
            await _unitOfWork.SaveChangesAsync();

            // Return a CreatedAtAction response with the newly created portfolio and its ID in the response headers.
            return CreatedAtAction(nameof(GetPortfolio), new { id = portfolio.PortfolioId }, portfolio);
        }

        // PUT: api/Portfolio/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePortfolio(int id,[FromBody] Portfolio portfolio)
        {
            portfolio.PortfolioId = id;
            if (id != portfolio.PortfolioId)
            {
                // If the provided ID does not match the ID in the portfolio object, return a BadRequest response.
                return BadRequest();
            }

            // Update the existing portfolio asynchronously in the repository through the Unit of Work.
            await _unitOfWork.PortfolioRepository.UpdatePortfolioAsync(portfolio);
            // Save the changes to the database asynchronously through the Unit of Work.
            await _unitOfWork.SaveChangesAsync();

            // Return a "204 No Content" status to indicate successful update without returning a response body.
            return Created(nameof(UpdatePortfolio), portfolio);
        }

        // DELETE: api/Portfolio/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePortfolio(string id)
        {
            // Get the portfolio by its ID asynchronously from the repository through the Unit of Work.
            var portfolio = await _unitOfWork.PortfolioRepository.GetPortfolioByIdAsync(id);
            if (portfolio == null)
            {
                // If the portfolio is not found, return a NotFound response.
                return NotFound();
            }

            // Delete the portfolio asynchronously from the repository through the Unit of Work.
            await _unitOfWork.PortfolioRepository.DeletePortfolioAsync(id);
            // Save the changes to the database asynchronously through the Unit of Work.
            await _unitOfWork.SaveChangesAsync();

            // Return a "204 No Content" status to indicate successful deletion without returning a response body.
            return NoContent();
        }
    }
}
