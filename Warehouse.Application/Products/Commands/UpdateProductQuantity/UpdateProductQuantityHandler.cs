using MediatR;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Commands.UpdateProductQuantity;

public class UpdateProductQuantityHandler : IRequestHandler<UpdateProductQuantityCommand, UpdateProductQuantityResponse?>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductQuantityHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<UpdateProductQuantityResponse?> Handle(
        UpdateProductQuantityCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);

        if (product == null)
            return null;

        product.UpdateQuantity(request.QuantityInStock);

        await _productRepository.UpdateAsync(product);

        return new UpdateProductQuantityResponse
        {
            Id = product.Id,
            Name = product.Name,
            QuantityInStock = product.QuantityInStock,
            LastUpdatedAt = product.LastUpdatedAt
        };
    }
}
