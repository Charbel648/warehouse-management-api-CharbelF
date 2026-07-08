using MediatR;

namespace Warehouse.Application.Products.Commands.AddProductImage;

public class AddProductImageCommand : IRequest<AddProductImageResponse?>
{
    public string ProductId { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;
}
