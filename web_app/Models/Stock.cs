using System.ComponentModel.DataAnnotations;

namespace web_app.Models;

public class Stock
{
    [Key]
    public int StockID { get; set; }

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
    // this is a navigation property that allows us to access the list of transactions associated with a particular stock
    public List<Transaction> Transactions { get; set; } = new ();
    public ICollection<StockHistory> StockHistories { get; set; }

    public Stock()
    {
        StockID = 0;
        Symbol = "";
        Name = "";
        Price = 0;
        PriceHigh = 0;
        PriceLow = 0;
        Date = "";
        Volume = 0;
    }
    
    public Stock(int stockId, string symbol, string name, decimal price, decimal priceLow, decimal priceHigh, string date, int volume)
    {
        StockID = stockId;
        Symbol = symbol;
        Name = name;
        Price = price;
        PriceLow = priceLow;
        PriceHigh = priceHigh;
        Date = date;
        Volume = volume;
    }
}