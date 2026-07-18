using AutoMapper;
using MediatR;
using Warehouse.Application.Common.Caching;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Queries.ListProducts;

public class ListProductsHandler : IRequestHandler<ListProductsQuery, List<ProductViewModel>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public ListProductsHandler(
        IProductRepository productRepository,
        IMapper mapper,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<List<ProductViewModel>> Handle(
        ListProductsQuery request,
        CancellationToken cancellationToken)
    {
        string cacheKey = CacheKeys.Products(request.OnlyAvailable);

        var cachedProducts = await _cacheService.GetAsync<List<ProductViewModel>>(
            cacheKey,
            cancellationToken);

        if (cachedProducts != null)
            return cachedProducts;

        var products = await _productRepository.GetAllAsync();

        if (request.OnlyAvailable)
        {
            products = products
                .Where(product => !product.IsArchived && product.QuantityInStock > 0)
                .ToList();
        }

        products = products
            .OrderByDescending(product => product.CreatedAt)
            .ToList();

        var result = _mapper.Map<List<ProductViewModel>>(products);

        await _cacheService.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(5),
            cancellationToken);

        return result;
    }
}
