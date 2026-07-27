namespace Warehouse.Application.Products.Queries.SearchProducts;

public class SearchProductsResponse
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int QuantityInStock { get; set; }

    public string SupplierName { get; set; } = string.Empty;
}

