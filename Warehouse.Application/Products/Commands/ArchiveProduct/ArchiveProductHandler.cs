using MediatR;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Commands.ArchiveProduct;

public class ArchiveProductHandler : IRequestHandler<ArchiveProductCommand, ArchiveProductResponse?>
{
    private readonly IProductRepository _productRepository;

    public ArchiveProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ArchiveProductResponse?> Handle(
        ArchiveProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);

        if (product == null)
            return null;

        product.Archive();

        await _productRepository.UpdateAsync(product);

        return new ArchiveProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            IsArchived = product.IsArchived,
            LastUpdatedAt = product.LastUpdatedAt
        };
    }
}

