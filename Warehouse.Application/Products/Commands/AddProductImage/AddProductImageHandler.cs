using MediatR;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Commands.AddProductImage;

public class AddProductImageHandler : IRequestHandler<AddProductImageCommand, AddProductImageResponse?>
{
    private readonly IProductRepository _productRepository;

    public AddProductImageHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<AddProductImageResponse?> Handle(
        AddProductImageCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);

        if (product == null)
            return null;

        ProductImage image = new ProductImage(
            request.ProductId,
            request.FileName,
            request.FilePath
        );

        product.AddImage(image);

        await _productRepository.UpdateAsync(product);

        return new AddProductImageResponse
        {
            ProductId = image.ProductId,
            FileName = image.FileName,
            FilePath = image.FilePath
        };
    }
}

