using System.ComponentModel.DataAnnotations;

namespace warehouse_management.Models;

public class ProductImage
{
    public string ProductId{get;set;}
    public string FileName { get; set; }
    public string FilePath  { get; set; }
}