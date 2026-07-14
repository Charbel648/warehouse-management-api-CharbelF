using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Products.Queries.ListProducts;

public class ListProductsHandler : IRequestHandler<ListProductsQuery, List<ProductViewModel>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ListProductsHandler(
        IProductRepository productRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<List<ProductViewModel>> Handle(
        ListProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync();

        if (request.OnlyAvailable)
        {
            products = products
                .Where(p => !p.IsArchived && p.QuantityInStock > 0)
                .ToList();
        }

        products = products
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return _mapper.Map<List<ProductViewModel>>(products);
    }
}
