using MediatR;

namespace Warehouse.Application.Products.Commands.ArchiveProduct;

public class ArchiveProductCommand : IRequest<ArchiveProductResponse?>
{
    public string ProductId { get; set; } = string.Empty;
}

