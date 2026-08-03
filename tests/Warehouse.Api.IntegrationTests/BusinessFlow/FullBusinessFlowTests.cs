using System.Net;
using System.Text.Json;
using FluentAssertions;
using Warehouse.Api.IntegrationTests.Helpers;
using Warehouse.Api.IntegrationTests.Infrastructure;

namespace Warehouse.Api.IntegrationTests.BusinessFlow;

public class FullBusinessFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public FullBusinessFlowTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task FullWarehouseBusinessFlow_ShouldSucceed()
    {
        string supplierId = await CreateSupplierAsync();

        string productId = await CreateProductAsync();

        var assignSupplierResponse = await _client.PostAsync(
            $"/api/products/{productId}/assign-supplier/{supplierId}",
            null);

        assignSupplierResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement assignJson = await ReadJsonAsync(assignSupplierResponse);
        GetString(assignJson, "productId").Should().Be(productId);
        GetString(assignJson, "supplierId").Should().Be(supplierId);

        using var imageForm = MultipartFormHelper.CreateFileForm(
            "Image",
            "business-flow-image.jpg",
            "image/jpeg",
            new byte[] { 1, 2, 3, 4 });

        var uploadImageResponse = await _client.PostAsync(
            $"/api/products/{productId}/image",
            imageForm);

        uploadImageResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement imageJson = await ReadJsonAsync(uploadImageResponse);
        GetString(imageJson, "relatedEntityId").Should().Be(productId);
        GetString(imageJson, "fileCategory").Should().Be("product-image");

        var updateQuantityResponse = await _client.PutAsync(
            $"/api/products/{productId}/quantity",
            JsonContentHelper.Create(new
            {
                QuantityInStock = 4
            }));

        updateQuantityResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement quantityJson = await ReadJsonAsync(updateQuantityResponse);
        GetInt(quantityJson, "quantityInStock").Should().Be(4);

        var updatePriceResponse = await _client.PutAsync(
            $"/api/products/{productId}/price",
            JsonContentHelper.Create(new
            {
                Price = 999.99m
            }));

        updatePriceResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement priceJson = await ReadJsonAsync(updatePriceResponse);
        GetDecimal(priceJson, "price").Should().Be(999.99m);

        var archiveResponse = await _client.DeleteAsync($"/api/products/{productId}");

        archiveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement archiveJson = await ReadJsonAsync(archiveResponse);
        GetBool(archiveJson, "isArchived").Should().BeTrue();

        var getArchivedProductResponse = await _client.GetAsync($"/api/products/{productId}");

        getArchivedProductResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement archivedProductJson = await ReadJsonAsync(getArchivedProductResponse);
        GetString(archivedProductJson, "id").Should().Be(productId);
        GetBool(archivedProductJson, "isArchived").Should().BeTrue();
    }

    private async Task<string> CreateSupplierAsync()
    {
        string suffix = Guid.NewGuid().ToString("N")[..8];

        var request = new CreateSupplierTestRequest
        {
            Name = $"Business Flow Supplier {suffix}",
            Country = "Lebanon",
            ContactEmail = $"business-flow-supplier-{suffix}@test.com",
            PhoneNumber = "+96100000000"
        };

        var response = await _client.PostAsync(
            "/api/suppliers",
            JsonContentHelper.Create(request));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        JsonElement json = await ReadJsonAsync(response);

        return GetString(json, "id")!;
    }

    private async Task<string> CreateProductAsync()
    {
        string suffix = Guid.NewGuid().ToString("N")[..8];

        var request = new CreateProductTestRequest
        {
            Name = $"Business Flow Product {suffix}",
            SKU = $"FLOW-{suffix}",
            Description = "Created for full business flow integration test",
            Price = 100,
            QuantityInStock = 10,
            SupplierName = "Business Flow Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(1)
        };

        var response = await _client.PostAsync(
            "/api/products",
            JsonContentHelper.Create(request));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        JsonElement json = await ReadJsonAsync(response);

        return GetString(json, "id")!;
    }

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
    {
        string json = await response.Content.ReadAsStringAsync();

        using JsonDocument document = JsonDocument.Parse(json);

        return document.RootElement.Clone();
    }

    private static string? GetString(JsonElement json, string propertyName)
    {
        return json.TryGetProperty(propertyName, out JsonElement value)
            ? value.GetString()
            : json.TryGetProperty(ToPascalCase(propertyName), out JsonElement pascalValue)
                ? pascalValue.GetString()
                : null;
    }

    private static int GetInt(JsonElement json, string propertyName)
    {
        return json.TryGetProperty(propertyName, out JsonElement value)
            ? value.GetInt32()
            : json.GetProperty(ToPascalCase(propertyName)).GetInt32();
    }

    private static decimal GetDecimal(JsonElement json, string propertyName)
    {
        return json.TryGetProperty(propertyName, out JsonElement value)
            ? value.GetDecimal()
            : json.GetProperty(ToPascalCase(propertyName)).GetDecimal();
    }

    private static bool GetBool(JsonElement json, string propertyName)
    {
        return json.TryGetProperty(propertyName, out JsonElement value)
            ? value.GetBoolean()
            : json.GetProperty(ToPascalCase(propertyName)).GetBoolean();
    }

    private static string ToPascalCase(string value)
    {
        return char.ToUpperInvariant(value[0]) + value[1..];
    }

    private class CreateSupplierTestRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string ContactEmail { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
    }

    private class CreateProductTestRequest
    {
        public string Name { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int QuantityInStock { get; set; }

        public string SupplierName { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }
    }
}
