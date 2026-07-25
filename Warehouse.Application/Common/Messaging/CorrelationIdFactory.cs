namespace Warehouse.Application.Common.Messaging;

public static class CorrelationIdFactory
{
    public static string Create()
    {
        return Guid.NewGuid().ToString();
    }
}
