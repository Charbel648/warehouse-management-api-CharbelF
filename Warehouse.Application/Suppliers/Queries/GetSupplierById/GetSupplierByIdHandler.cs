using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdHandler : IRequestHandler<GetSupplierByIdQuery, SupplierViewModel?>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;

    public GetSupplierByIdHandler(
        ISupplierRepository supplierRepository,
        IMapper mapper)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<SupplierViewModel?> Handle(
        GetSupplierByIdQuery request,
        CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId);

        if (supplier == null)
            return null;

        return _mapper.Map<SupplierViewModel>(supplier);
    }
}
