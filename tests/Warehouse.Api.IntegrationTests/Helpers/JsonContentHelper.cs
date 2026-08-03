using System.Net.Http.Json;

namespace Warehouse.Api.IntegrationTests.Helpers;

public static class JsonContentHelper
{
    public static JsonContent Create(object value)
    {
        return JsonContent.Create(value);
    }
}
