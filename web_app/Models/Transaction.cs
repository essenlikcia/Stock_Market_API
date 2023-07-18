using System.ComponentModel.DataAnnotations;

namespace web_app.Models;

public class Transaction
{

    [Required]
    public int StockId { get; set; }

    [Required]
    public string TransactionType { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime TransactionDate { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public double Price { get; set; }

    // Navigation property to relate to the Stock model
    public Stock Stock { get; set; }

    // Navigation property to relate to the User model
    public CustomUser User { get; set; }
    [Key]
    public string Id { get; set; }

    public Transaction()
    {
        Id = "";
        StockId = 0;
        TransactionType = "";
        TransactionDate = DateTime.Now;
        Quantity = 0;
        Price = 0;
    }

    public Transaction(string Id, int stockId, string transactionType, DateTime transactionDate, int quantity,
        double price)
    {
        Id = Id;
        StockId = stockId;
        TransactionType = transactionType;
        TransactionDate = transactionDate;
        Quantity = quantity;
        Price = price;
    }
}