using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Commands;

public class AddProductImage
{
    private readonly IProductRepository _productRepository;

    public AddProductImage(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductImage?> ExecuteAsync(
        string productId,
        string fileName,
        string filePath)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null)
        {
            return null;
        }

        ProductImage image = new ProductImage(productId, fileName, filePath);

        product.AddImage(image);

        await _productRepository.UpdateAsync(product);

        return image;
    }
}