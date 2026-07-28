using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Warehouse.Api.IntegrationTests.Helpers;
using Warehouse.Api.IntegrationTests.Infrastructure;

namespace Warehouse.Api.IntegrationTests.Products;

public class ProductEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnSuccess()
    {
        var response = await _client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string json = await response.Content.ReadAsStringAsync();
        json.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetProduct_WithExistingId_ShouldReturnProduct()
    {
        string productId = await CreateProductAsync();

        var response = await _client.GetAsync($"/api/products/{productId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement json = await ReadJsonAsync(response);
        GetString(json, "id").Should().Be(productId);
    }

    [Fact]
    public async Task GetProduct_WithMissingId_ShouldReturnNotFound()
    {
        var missingId = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/products/{missingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchProducts_ByName_ShouldReturnMatches()
    {
        string uniqueName = "Integration Product " + Guid.NewGuid().ToString("N")[..8];

        await CreateProductAsync(uniqueName);

        var response = await _client.GetAsync($"/api/products/search?name={Uri.EscapeDataString(uniqueName)}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string json = await response.Content.ReadAsStringAsync();
        json.Should().Contain(uniqueName);
    }

    [Fact]
    public async Task CreateProduct_WithValidRequest_ShouldReturnCreated()
    {
        var request = CreateProductRequest();

        var response = await _client.PostAsync(
            "/api/products",
            JsonContentHelper.Create(request));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        JsonElement json = await ReadJsonAsync(response);

        GetString(json, "id").Should().NotBeNullOrWhiteSpace();
        GetString(json, "name").Should().Be(request.Name);
        GetString(json, "sku").Should().Be(request.SKU);
    }

    [Fact]
    public async Task CreateProduct_WithDuplicateSku_ShouldReturnConflict()
    {
        string sku = "SKU-" + Guid.NewGuid().ToString("N")[..8];

        var firstRequest = CreateProductRequest(sku: sku);
        var secondRequest = CreateProductRequest(sku: sku);

        var firstResponse = await _client.PostAsync(
            "/api/products",
            JsonContentHelper.Create(firstRequest));

        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var secondResponse = await _client.PostAsync(
            "/api/products",
            JsonContentHelper.Create(secondRequest));

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateQuantity_ShouldReturnUpdatedProduct()
    {
        string productId = await CreateProductAsync();

        var response = await _client.PutAsync(
            $"/api/products/{productId}/quantity",
            JsonContentHelper.Create(new
            {
                QuantityInStock = 3
            }));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement json = await ReadJsonAsync(response);
        GetInt(json, "quantityInStock").Should().Be(3);
    }

    [Fact]
    public async Task UpdatePrice_ShouldReturnUpdatedProduct()
    {
        string productId = await CreateProductAsync();

        var response = await _client.PutAsync(
            $"/api/products/{productId}/price",
            JsonContentHelper.Create(new
            {
                Price = 250.75m
            }));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement json = await ReadJsonAsync(response);
        GetDecimal(json, "price").Should().Be(250.75m);
    }

    [Fact]
    public async Task DeleteProduct_ShouldArchiveProduct()
    {
        string productId = await CreateProductAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/products/{productId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement deleteJson = await ReadJsonAsync(deleteResponse);
        GetBool(deleteJson, "isArchived").Should().BeTrue();

        var getResponse = await _client.GetAsync($"/api/products/{productId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement getJson = await ReadJsonAsync(getResponse);
        GetBool(getJson, "isArchived").Should().BeTrue();
    }

    private async Task<string> CreateProductAsync(string? name = null, string? sku = null)
    {
        var request = CreateProductRequest(name, sku);

        var response = await _client.PostAsync(
            "/api/products",
            JsonContentHelper.Create(request));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        JsonElement json = await ReadJsonAsync(response);

        return GetString(json, "id")!;
    }

    private static CreateProductTestRequest CreateProductRequest(
        string? name = null,
        string? sku = null)
    {
        string suffix = Guid.NewGuid().ToString("N")[..8];

        return new CreateProductTestRequest
        {
            Name = name ?? $"Integration Product {suffix}",
            SKU = sku ?? $"SKU-{suffix}",
            Description = "Created from integration test",
            Price = 100.50m,
            QuantityInStock = 10,
            SupplierName = "Integration Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(1)
        };
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
