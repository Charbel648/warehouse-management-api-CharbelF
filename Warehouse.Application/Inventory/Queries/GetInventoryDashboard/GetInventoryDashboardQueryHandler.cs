using MediatR;
using Warehouse.Application.Common.Caching;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Inventory.Queries.GetInventoryDashboard;

public class GetInventoryDashboardQueryHandler
    : IRequestHandler<GetInventoryDashboardQuery, InventoryDashboardViewModel>
{
    private readonly IInventoryDashboardRepository _inventoryDashboardRepository;
    private readonly ICacheService _cacheService;

    public GetInventoryDashboardQueryHandler(
        IInventoryDashboardRepository inventoryDashboardRepository,
        ICacheService cacheService)
    {
        _inventoryDashboardRepository = inventoryDashboardRepository;
        _cacheService = cacheService;
    }

    public async Task<InventoryDashboardViewModel> Handle(
        GetInventoryDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var cachedDashboard = await _cacheService.GetAsync<InventoryDashboardViewModel>(
            CacheKeys.InventoryDashboardGraphs,
            cancellationToken);

        if (cachedDashboard != null)
            return cachedDashboard;

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

        await _cacheService.SetAsync(
            CacheKeys.InventoryDashboardGraphs,
            dashboard,
            TimeSpan.FromMinutes(5),
            cancellationToken);

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


