using System.ComponentModel.DataAnnotations;

namespace web_app.Models;

public class Portfolio
{
    [Key]
    public int PortfolioId { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public decimal InitialBalance { get; set; }
    [Required]
    public decimal CurrentBalance { get; set; }
    [Required]
    public decimal TotalProfit { get; set; }
    [Required]
    public decimal TotalProfitPercentage { get; set; }
    [Required]
    public decimal Quantity { get; set; }
    // navigation property, establishes a relationship between portfolio entity and user entity
    public CustomUser User { get; set; }

    public Portfolio()
    {
        PortfolioId = 0;
        UserId = "";
        Name = "";
        Description = "";
        InitialBalance = 0;
        CurrentBalance = 0;
        TotalProfit = 0;
        TotalProfitPercentage = 0;
        Quantity = 0;
    }
    public Portfolio(int portfolioId, string userId, string name, string description, decimal initialBalance,
        decimal currentBalance, decimal totalProfit, decimal totalProfitPercentage, decimal quantity)
    {
        PortfolioId = portfolioId;
        UserId = userId;
        Name = name;
        Description = description;
        InitialBalance = initialBalance;
        CurrentBalance = currentBalance;
        TotalProfit = totalProfit;
        TotalProfitPercentage = totalProfitPercentage;
        Quantity = quantity;
    }
    
    // override ToString() method to return a string representation of the object
    public override string ToString()
    {
        return $"PortfolioId: {PortfolioId}, UserId: {UserId}, Name: {Name}, Description: {Description}," +
               $" InitialBalance: {InitialBalance}, CurrentBalance: {CurrentBalance}, TotalProfit: {TotalProfit}," +
               $" TotalProfitPercentage: {TotalProfitPercentage}, Quantity: {Quantity}";
    }
    // override Equals() method to return true if the objects are equal
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        Portfolio p = (Portfolio) obj;
        return PortfolioId.Equals(p.PortfolioId);
    }
}