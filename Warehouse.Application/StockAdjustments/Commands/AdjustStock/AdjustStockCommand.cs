using MediatR;

namespace Warehouse.Application.StockAdjustments.Commands.AdjustStock;

public class AdjustStockCommand : IRequest<AdjustStockResponse>
{
    public string ProductId { get; set; } = string.Empty;

    public int QuantityChange { get; set; }

    public string Reason { get; set; } = string.Empty;
}
