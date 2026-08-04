using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Api.IntegrationTests.Helpers;
using Warehouse.Api.IntegrationTests.Infrastructure;

namespace Warehouse.Api.IntegrationTests.AiGenerated;

public class AiGeneratedProductFlowIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWarehouseStore _store;

    public AiGeneratedProductFlowIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _store = factory.Services.GetRequiredService<TestWarehouseStore>();
    }

    [Fact]
    public async Task PostProduct_ShouldReturnCreatedHeaderPropertiesAndPersistProduct()
    {
        string suffix = Guid.NewGuid().ToString("N")[..8];

        var request = new CreateProductTestRequest
        {
            Name = $"AI Product {suffix}",
            SKU = $"AI-SKU-{suffix}",
            Description = "Created by Exercise 04 integration test",
            Price = 150.25m,
            QuantityInStock = 12,
            SupplierName = "AI Supplier",
            ExpiryDate = DateTime.UtcNow.AddYears(1)
        };

        var response = await _client.PostAsync(
            "/api/products",
            JsonContentHelper.Create(request));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        JsonElement json = await ReadJsonAsync(response);

        string productId = GetString(json, "id")!;

        productId.Should().NotBeNullOrWhiteSpace();
        GetString(json, "name").Should().Be(request.Name);
        GetString(json, "sku").Should().Be(request.SKU);
        GetDecimal(json, "price").Should().Be(request.Price);
        GetInt(json, "quantityInStock").Should().Be(request.QuantityInStock);

        var persistedProduct = _store.Products.FirstOrDefault(product => product.Id == productId);

        persistedProduct.Should().NotBeNull();
        persistedProduct!.Name.Should().Be(request.Name);
        persistedProduct.SKU.Should().Be(request.SKU);
        persistedProduct.IsArchived.Should().BeFalse();
    }

    [Fact]
    public async Task PostProductImage_ShouldReturnOkPropertiesAndPersistFileMetadata()
    {
        string productId = await CreateProductAsync();

        using var form = MultipartFormHelper.CreateFileForm(
            "Image",
            "ai-product-image.jpg",
            "image/jpeg",
            new byte[] { 1, 2, 3, 4, 5 });

        var response = await _client.PostAsync($"/api/products/{productId}/image", form);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType!.MediaType.Should().Contain("json");

        JsonElement json = await ReadJsonAsync(response);

        string fileId = GetString(json, "fileId")!;

        fileId.Should().NotBeNullOrWhiteSpace();
        GetString(json, "relatedEntityId").Should().Be(productId);
        GetString(json, "relatedEntityType").Should().Be("product");
        GetString(json, "fileCategory").Should().Be("product-image");
        GetString(json, "originalFileName").Should().Be("ai-product-image.jpg");
        GetString(json, "contentType").Should().Be("image/jpeg");

        var persistedFile = _store.Files.FirstOrDefault(file => file.FileId == fileId);

        persistedFile.Should().NotBeNull();
        persistedFile!.RelatedEntityId.Should().Be(productId);
        persistedFile.RelatedEntityType.Should().Be("product");
        persistedFile.FileCategory.Should().Be("product-image");
        persistedFile.OriginalFileName.Should().Be("ai-product-image.jpg");
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnOkArchiveProductAndKeepItPersisted()
    {
        string productId = await CreateProductAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/products/{productId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        deleteResponse.Content.Headers.ContentType.Should().NotBeNull();
        deleteResponse.Content.Headers.ContentType!.MediaType.Should().Contain("json");

        JsonElement deleteJson = await ReadJsonAsync(deleteResponse);

        GetString(deleteJson, "id").Should().Be(productId);
        GetBool(deleteJson, "isArchived").Should().BeTrue();

        var persistedProduct = _store.Products.FirstOrDefault(product => product.Id == productId);

        persistedProduct.Should().NotBeNull();
        persistedProduct!.IsArchived.Should().BeTrue();

        var getResponse = await _client.GetAsync($"/api/products/{productId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement getJson = await ReadJsonAsync(getResponse);

        GetString(getJson, "id").Should().Be(productId);
        GetBool(getJson, "isArchived").Should().BeTrue();
    }

    private async Task<string> CreateProductAsync()
    {
        string suffix = Guid.NewGuid().ToString("N")[..8];

        var request = new CreateProductTestRequest
        {
            Name = $"AI Flow Product {suffix}",
            SKU = $"AI-FLOW-{suffix}",
            Description = "Created by helper for Exercise 04",
            Price = 100,
            QuantityInStock = 10,
            SupplierName = "AI Flow Supplier",
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
