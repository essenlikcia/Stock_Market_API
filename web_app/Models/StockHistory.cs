using System.ComponentModel.DataAnnotations;

namespace web_app.Models;

public class StockHistory
{
    //guid
    [Key]
    public int StockHistoryID { get; set; }

    [Required]
    [StringLength(50)]
    public string Symbol { get; set; }

    [Required]
    [StringLength(50)]
    public string? Name { get; set; }

    [Required]
    public decimal Price { get; set; }
    [Required]
    public decimal PriceLow { get; set; }
    [Required]
    public decimal PriceHigh { get; set; }
    [Required]
    public string Date { get; set; }
    [Required]
    public int Volume { get; set; }

    // Navigation property
    public int StockId { get; set; } // Foreign key to Stock entity
    public Stock Stock { get; set; } // Navigation property to Stock entity

    public StockHistory()
    {
        StockHistoryID = 0;
        Symbol = "";
        Name = "";
        Price = 0;
        PriceHigh = 0;
        PriceLow = 0;
        Date = "";
        Volume = 0;
    }

    public StockHistory(int stockId, string symbol, string name, decimal price, decimal priceLow, decimal priceHigh, string date, int volume)
    {
        StockHistoryID = stockId;
        Symbol = symbol;
        Name = name;
        Price = price;
        PriceLow = priceLow;
        PriceHigh = priceHigh;
        Date = date;
        Volume = volume;
    }
}