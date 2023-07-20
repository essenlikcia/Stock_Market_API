using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using web_app.Core.Repositories;
using web_app.Data;
using web_app.Models;
using System.Net.Http;
using System.ComponentModel.Design;

namespace web_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;

        public StockController(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory)
        {
            _unitOfWork = unitOfWork;
            _httpClientFactory = httpClientFactory;
        }

        // GET: /api/Stock
        // retrieve all stocks from the database
        [HttpGet]
        public async Task<IActionResult> GetStocks()    
        {
            try
            {
                var stocks = await _unitOfWork.StockRepository.GetStocksAsync();
                return Ok(stocks);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: /api/Stock/5
        // Retrieves a specific stock by its ID from the database.
        [HttpGet("{id}")]
        public async Task<ActionResult<Stock>> GetStock(string id)
        {
            var stock = await _unitOfWork.StockRepository.GetStockByIdAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock);
        }

        // POST: /api/Stock
        // Adds a new stock to the database
        // AddStock is designed to return a generic IActionResult
        [HttpPost]
        public async Task<IActionResult> AddStock([FromBody] Stock stock)
        {
            try
            {
                await _unitOfWork.StockRepository.AddStockAsync(stock);
                await _unitOfWork.SaveChangesAsync();

                return CreatedAtAction(nameof(GetStocks), new { id = stock.StockID }, stock);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: /api/Stock/5
        // Updates an existing stock in the database.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] Stock stock)
        {
            try
            {
                if (id != stock.StockID)
                    return BadRequest("Invalid stock ID.");

                await _unitOfWork.StockRepository.UpdateStockAsync(stock);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: /api/Stock/5
        // Deletes a specific stock by its ID from the database.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(string id)
        {
            try
            {
                await _unitOfWork.StockRepository.DeleteStockAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /*
        public async Task<IActionResult> Index()
        {
            // Create an instance of HttpClient
            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Replace the URL with the actual API endpoint that provides stock data
                    string apiUrl = "https://bigpara.hurriyet.com.tr/borsa/canli-borsa/";

                    // Fetch the data from the API
                    var response = await httpClient.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    var jsonString = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON response into a list of Stock objects
                    var stocks = JsonConvert.DeserializeObject<List<Stock>>(jsonString);

                    return View(stocks);
                }
                catch (Exception ex)
                {
                    // Handle any errors here
                    return View("Error");
                }
            }
        }*/
    }
}