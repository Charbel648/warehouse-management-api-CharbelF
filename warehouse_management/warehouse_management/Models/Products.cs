using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace warehouse_management.Models;


public class Products
{
    [Key]
    public string Id{ get; set; }
    [Required]
    [MaxLength(50)]
    public string Name  { get; set; }
    public string? SKU   { get; set; }
    public string? Description { get; set; }
    [Required]
    public double Price  { get; set; }
    public int QuantityInStock  { get; set; }
    public string? SupplierName { get; set; }
    public DateTime ExpiryDate  { get; set; }
    public bool IsArchived  { get; set; }
    public DateTime Created_at   { get; set; }
    public DateTime Last_Updated_at { get; set; }
    
    public Products(string Id, string Name, string SKU, string Description, double Price, int QuantityInStock,string supplierName, DateTime ExpiryDate, DateTime Created_at, DateTime Last_Updated_at)
    {
        Id = this.Id;
        Name = this.Name;
        Description = this.Description;
        SKU = this.SKU;
        Price = this.Price;
        QuantityInStock = this.QuantityInStock;
        supplierName = this.SupplierName;
        Created_at = this.Created_at;
        Last_Updated_at = this.Last_Updated_at;
        IsArchived = false;
    }
}