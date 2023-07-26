using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using web_app.Core.Repositories;
using web_app.Models;
using AlphaVantage.Net.Core.Client;
using AlphaVantage.Net.Stocks;
using AlphaVantage.Net.Stocks.Client;
using Ecng.Common;

namespace web_app.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StockHistoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AlphaVantageClient _alphaVantageClient;

        private readonly TimeSpan _updateInterval = TimeSpan.FromHours(4); // Update stock data every 4 hours
        private Timer _timer;

        public StockHistoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            string apiKey = "UCTK723VT0KPF10W";
            _alphaVantageClient = new AlphaVantageClient(apiKey);
            _timer = new Timer(UpdateStockHistoryData, null, _updateInterval, _updateInterval);
        }

        public class StockSymbolsRequest
        {
            public List<string> Symbols { get; set; }
        }

        // POST: /api/StockHistory/GetStockData
        // Retrieves historical stock data from AlphaVantage API for multiple symbols and adds to the historical data table.
        [HttpPost]
        public async Task<IActionResult> GetStockHistoryData([FromBody] StockSymbolsRequest request)
        {
            try
            {
                var stockHistoryList = new List<StockHistory>();

                // Use AlphaVantage client to fetch historical stock data for each symbol in the list
                foreach (var symbol in request.Symbols)
                {
                    var stockData = await _alphaVantageClient.Stocks().GetGlobalQuoteAsync(symbol);
                    if (stockData != null)
                    {
                        var stockHistory = new StockHistory
                        {
                            Symbol = stockData.Symbol,
                            Price = stockData.Price,
                            PriceLow = stockData.LowestPrice,
                            PriceHigh = stockData.HighestPrice,
                            Date = DateTime.Now.ToString(),
                            Volume = (int)stockData.Volume
                        };
                        stockHistoryList.Add(stockHistory);
                    }
                }

                // Save historical stock data in the database
                await _unitOfWork.StockHistoryRepository.AddStockHistoriesAsync(stockHistoryList);
                await _unitOfWork.SaveChangesAsync();

                return Ok(stockHistoryList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Background task to update stock history data at regular intervals
        private async void UpdateStockHistoryData(object state)
        {
            try
            {
                // Fetch data for all stored symbols
                var storedStockHistory = await _unitOfWork.StockHistoryRepository.GetStocksHistoryAsync();
                var symbolsToUpdate = storedStockHistory.Select(s => s.Symbol).ToList();

                // Create the StockSymbolsRequest object and set the Symbols property
                var request = new StockSymbolsRequest
                {
                    Symbols = symbolsToUpdate
                };

                // Fetch and store updated data
                await GetStockHistoryData(request);
            }
            catch (Exception ex)
            {
                // Log or handle any errors that occur during the background update
                Console.WriteLine($"Error updating stock history data: {ex.Message}");
            }
        }

        // GET: /api/StockHistory
        // Retrieve all historical stock data from the database
        [HttpGet]
        public async Task<IActionResult> GetStockHistory()
        {
            try
            {
                var stockHistories = await _unitOfWork.StockHistoryRepository.GetStocksHistoryAsync();
                return Ok(stockHistories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: /api/StockHistory/5
        // Retrieves a specific historical stock data by its ID from the database.
        [HttpGet("{id}")]
        public async Task<ActionResult<StockHistory>> GetStockHistory(string id)
        {
            var stockHistory = await _unitOfWork.StockHistoryRepository.GetStockHistoryByIdAsync(id);
            if (stockHistory == null)
            {
                return NotFound();
            }
            return Ok(stockHistory);
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
