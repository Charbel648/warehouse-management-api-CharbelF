using MediatR;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Inventory.Queries.GetInventoryDashboard;

public class GetInventoryDashboardQueryHandler
    : IRequestHandler<GetInventoryDashboardQuery, InventoryDashboardViewModel>
{
    private readonly IInventoryDashboardRepository _inventoryDashboardRepository;

    public GetInventoryDashboardQueryHandler(
        IInventoryDashboardRepository inventoryDashboardRepository)
    {
        _inventoryDashboardRepository = inventoryDashboardRepository;
    }

    public async Task<InventoryDashboardViewModel> Handle(
        GetInventoryDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var dashboard = new InventoryDashboardViewModel();

        var productStatusGraph = await BuildGraphAsync(
            "Products by status",
            () => _inventoryDashboardRepository.GetProductStatusGraphAsync(cancellationToken));

        dashboard.Graphs.Add(productStatusGraph);

        var stockLevelGraph = await BuildGraphAsync(
            "Products by stock level",
            () => _inventoryDashboardRepository.GetStockLevelGraphAsync(cancellationToken));

        dashboard.Graphs.Add(stockLevelGraph);

        var suppliersByCountryGraph = await BuildGraphAsync(
            "Suppliers by country",
            () => _inventoryDashboardRepository.GetSuppliersByCountryGraphAsync(cancellationToken));

        dashboard.Graphs.Add(suppliersByCountryGraph);

        return dashboard;
    }

    private static async Task<InventoryDashboardGraphViewModel> BuildGraphAsync(
        string graphName,
        Func<Task<List<DashboardGraphPoint>>> getGraphData)
    {
        try
        {
            var points = await getGraphData();

            return new InventoryDashboardGraphViewModel
            {
                Name = graphName,
                IsAvailable = true,
                Points = points.Select(point => new InventoryDashboardGraphPointViewModel
                {
                    Label = point.Label,
                    Value = point.Value
                }).ToList()
            };
        }
        catch
        {
            return new InventoryDashboardGraphViewModel
            {
                Name = graphName,
                IsAvailable = false,
                ErrorMessage = "This graph is temporarily unavailable"
            };
        }
    }
}
