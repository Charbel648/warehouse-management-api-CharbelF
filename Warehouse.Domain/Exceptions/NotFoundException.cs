namespace Warehouse.Domain.Exceptions;

public class NotFoundException : Exception
{
    public string ResourceName { get; }

    public string ResourceId { get; }

    public NotFoundException(string resourceName, string resourceId)
        : base($"{resourceName} with id '{resourceId}' was not found")
    {
        ResourceName = resourceName;
        ResourceId = resourceId;
    }
}
