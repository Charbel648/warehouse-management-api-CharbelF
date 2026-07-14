using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Warehouse.Application.Common;

namespace Warehouse.Presentation.Services;

public class ValidationMetadataService
{
    public Result<object> GetValidationMetadata(string dtoName)
    {
        Type? dtoType = typeof(Contracts.CreateProductRequest)
            .Assembly
            .GetTypes()
            .Where(type => type.Namespace == "Warehouse.Presentation.Contracts")
            .FirstOrDefault(type =>
                type.Name.Equals(dtoName, StringComparison.OrdinalIgnoreCase)
                || type.Name.Equals($"{dtoName}Request", StringComparison.OrdinalIgnoreCase));

        if (dtoType == null)
            return Result<object>.Failure($"DTO '{dtoName}' was not found");

        var properties = dtoType
            .GetProperties()
            .Select(property => new
            {
                Name = property.Name,
                Type = GetReadableTypeName(property.PropertyType),
                IsRequired = property.GetCustomAttribute<RequiredAttribute>() != null,
                ValidationAttributes = property
                    .GetCustomAttributes<ValidationAttribute>()
                    .Select(attribute => new
                    {
                        Name = attribute.GetType().Name.Replace("Attribute", ""),
                        attribute.ErrorMessage
                    })
                    .ToList()
            })
            .ToList();

        object metadata = new
        {
            DtoName = dtoType.Name,
            UsesCrossPropertyValidation = typeof(IValidatableObject).IsAssignableFrom(dtoType),
            Properties = properties
        };

        return Result<object>.Success(metadata);
    }

    private static string GetReadableTypeName(Type type)
    {
        Type? nullableType = Nullable.GetUnderlyingType(type);

        if (nullableType != null)
            return $"{nullableType.Name}?";

        return type.Name;
    }
}
