using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Suppliers.Queries.ListSuppliers;

public class ListSuppliersHandler : IRequestHandler<ListSuppliersQuery, List<SupplierViewModel>>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;

    public ListSuppliersHandler(
        ISupplierRepository supplierRepository,
        IMapper mapper)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<List<SupplierViewModel>> Handle(
        ListSuppliersQuery request,
        CancellationToken cancellationToken)
    {
        var suppliers = await _supplierRepository.GetAllAsync();

        return _mapper.Map<List<SupplierViewModel>>(suppliers);
    }
}

