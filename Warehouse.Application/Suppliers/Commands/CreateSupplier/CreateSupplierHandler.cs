using MediatR;
using Warehouse.Domain.Models;
using Warehouse.Domain.Repositories;

namespace Warehouse.Application.Suppliers.Commands.CreateSupplier;

public class CreateSupplierHandler : IRequestHandler<CreateSupplierCommand, CreateSupplierResponse>
{
    private readonly ISupplierRepository _supplierRepository;

    public CreateSupplierHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<CreateSupplierResponse> Handle(
        CreateSupplierCommand request,
        CancellationToken cancellationToken)
    {
        bool emailExists = await _supplierRepository.EmailExistsAsync(request.ContactEmail);

        if (emailExists)
            throw new InvalidOperationException("A supplier with the same email already exists");

        Supplier supplier = new Supplier(
            request.Name,
            request.Country,
            request.ContactEmail,
            request.PhoneNumber
        );

        await _supplierRepository.AddAsync(supplier);

        return new CreateSupplierResponse
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Country = supplier.Country,
            ContactEmail = supplier.ContactEmail,
            PhoneNumber = supplier.PhoneNumber,
            IsActive = supplier.IsActive
        };
    }
}

