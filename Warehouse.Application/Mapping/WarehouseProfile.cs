using AutoMapper;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Models;

namespace Warehouse.Application.Mapping;

public class WarehouseProfile : Profile
{
    public WarehouseProfile()
    {
        CreateMap<Product, ProductViewModel>()
            .ForMember(destination => destination.Id,
                option => option.MapFrom(source => source.ProductId))
            .ForMember(destination => destination.SupplierId,
                option => option.MapFrom(source => source.SupplierId ?? string.Empty))
            .ForMember(destination => destination.SupplierName,
                option => option.MapFrom(source =>
                    source.Supplier != null ? source.Supplier.Name : source.SupplierName))
            .ForMember(destination => destination.ImageUrls,
                option => option.MapFrom(source =>
                    source.Images.Select(image => image.FilePath).ToList()));

        CreateMap<Supplier, SupplierViewModel>()
            .ForMember(destination => destination.Id,
                option => option.MapFrom(source => source.SupplierId))
            .ForMember(destination => destination.ProductCount,
                option => option.MapFrom(source => source.Products.Count));
    }
}

