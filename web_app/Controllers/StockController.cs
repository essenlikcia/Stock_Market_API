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
using AlphaVantage.Net.Core.Client;
using AlphaVantage.Net.Stocks;
using AlphaVantage.Net.Stocks.Client;
using Microsoft.AspNetCore.Authorization;

namespace web_app.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StockController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AlphaVantageClient _alphaVantageClient;

        private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(1); // Update stock data every minute
        private Timer _timer;

        public StockController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            string apiKey = "UCTK723VT0KPF10W";
            _alphaVantageClient = new AlphaVantageClient(apiKey);
            _timer = new Timer(UpdateStockData, null, _updateInterval, _updateInterval);
        }

        public class StockSymbolsRequest
        {
            public List<string> Symbols { get; set; }
        }

        // POST: /api/Stock/GetStockData
        // Retrieves stock data from AlphaVantage API for multiple symbols.
        [HttpPost]
        public async Task<IActionResult> GetStockData([FromBody]StockSymbolsRequest request)
        {
            try
            {
                var stockDataList = new List<Stock>();

                // Use AlphaVantage client to fetch stock data for each symbol in the list
                foreach (var symbol in request.Symbols)
                {
                    var stockData = await _alphaVantageClient.Stocks().GetGlobalQuoteAsync(symbol);
                    if (stockData != null)
                    {
                        var stock = new Stock
                        {
                            Symbol = stockData.Symbol,
                            Price = stockData.Price,
                            PriceLow = stockData.LowestPrice,
                            PriceHigh = stockData.HighestPrice,
                            Date = DateTime.Now.ToString(),
                            Volume = (int)stockData.Volume
                        };
                        stockDataList.Add(stock);
                    }
                }

                // Save or update stock data in the database
                foreach (var stockData in stockDataList)
                {
                    var existingStock = await _unitOfWork.StockRepository.GetStockBySymbolAsync(stockData.Symbol);

                    if (existingStock != null)
                    {
                        // Update existing stock data
                        existingStock.Price = stockData.Price;
                        existingStock.PriceLow = stockData.PriceLow;
                        existingStock.PriceHigh = stockData.PriceHigh;
                        existingStock.Date = stockData.Date;
                        existingStock.Volume = stockData.Volume;

                        await _unitOfWork.StockRepository.UpdateStockAsync(existingStock);
                    }
                    else
                    {
                        // Add new stock data
                        await _unitOfWork.StockRepository.AddStockAsync(stockData);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
                return Ok(stockDataList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Background task to update stock data at regular intervals
        private async void UpdateStockData(object state)
        {
            try
            {
                // Fetch data for all stored symbols
                var storedStocks = await _unitOfWork.StockRepository.GetStocksAsync();
                var symbolsToUpdate = storedStocks.Select(s => s.Symbol).ToList();

                // Create the StockSymbolsRequest object and set the Symbols property
                var request = new StockSymbolsRequest
                {
                    Symbols = symbolsToUpdate
                };

                // Fetch and store updated data
                await GetStockData(request);
            }
            catch (Exception ex)
            {
                // Log or handle any errors that occur during the background update
                Console.WriteLine($"Error updating stock data: {ex.Message}");
            }
        }
        public class TradingStatusRequest
        {
            public string Symbol;
            public bool IsTradingAllowed;
        }

        // POST: /api/Stock/SetTradingStatus
        // Allows the Admin to set the trading status for specific stocks.
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> SetTradingStatus([FromBody] TradingStatusRequest request)
        {
            try
            {
                // Get the stock by its symbol asynchronously from the repository through the Unit of Work.
                var stock = await _unitOfWork.StockRepository.GetStockBySymbolAsync(request.Symbol);
                if (stock == null)
                {
                    return NotFound();
                }

                // Update the IsTradingAllowed property of the stock and save the changes.
                stock.isTradingAllowed = request.IsTradingAllowed;
                await _unitOfWork.StockRepository.UpdateStockAsync(stock);
                await _unitOfWork.SaveChangesAsync();

                return Ok($"Trading status for stock {stock.Symbol} has been set to {(request.IsTradingAllowed ? "Allowed" : "Stopped")}.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

        /* doesn't work */
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
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateStock([FromQuery]int id, [FromBody] Stock stock)
        {
            try
            {
                stock.StockID = id;
                if (id != stock.StockID)
                    return BadRequest("Invalid stock ID.");

                await _unitOfWork.StockRepository.UpdateStockAsync(stock);
                await _unitOfWork.SaveChangesAsync();

                return Created(nameof(UpdateStock), stock);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: /api/Stock/5
        // Deletes a specific stock by its ID from the database.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock([FromQuery]string id)
        {
            // Get the portfolio by its ID asynchronously from the repository through the Unit of Work.
            var stock = await _unitOfWork.StockRepository.GetStockByIdAsync(id);
            if (stock == null)
            {
                // If the portfolio is not found, return a NotFound response.
                return NotFound();
            }

            // Delete the portfolio asynchronously from the repository through the Unit of Work.
            await _unitOfWork.StockRepository.DeleteStockByIdAsync(id);
            // Save the changes to the database asynchronously through the Unit of Work.
            await _unitOfWork.SaveChangesAsync();

            // Return a "204 No Content" status to indicate successful deletion without returning a response body.
            return NoContent();
        }

        // Dispose method is overridden to release resources when the controller is no longer needed.
        protected override void Dispose(bool disposing)
        {
            // Dispose the _timer object if it's not null.
            // The ? is the null-conditional operator, ensuring the Dispose method is only called if _timer is not null.
            _timer?.Dispose();

            // Call the base Dispose method to perform any cleanup tasks defined in the base Controller class.
            base.Dispose(disposing);
        }
    }
}