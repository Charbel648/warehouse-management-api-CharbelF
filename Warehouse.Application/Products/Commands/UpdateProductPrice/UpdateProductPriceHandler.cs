using MediatR;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Commands.UpdateProductPrice;

public class UpdateProductPriceHandler : IRequestHandler<UpdateProductPriceCommand, UpdateProductPriceResponse?>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductPriceHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<UpdateProductPriceResponse?> Handle(
        UpdateProductPriceCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);

        if (product == null)
            return null;

        product.UpdatePrice(request.Price);

        await _productRepository.UpdateAsync(product);

        return new UpdateProductPriceResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            LastUpdatedAt = product.LastUpdatedAt
        };
    }
}

