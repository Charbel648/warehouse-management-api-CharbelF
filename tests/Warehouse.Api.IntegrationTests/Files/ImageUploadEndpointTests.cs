using System.Net;
using System.Text.Json;
using FluentAssertions;
using Warehouse.Api.IntegrationTests.Helpers;
using Warehouse.Api.IntegrationTests.Infrastructure;

namespace Warehouse.Api.IntegrationTests.Files;

public class ImageUploadEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ImageUploadEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UploadProductImage_WithJpg_ShouldReturnSuccess()
    {
        string productId = await CreateProductAsync();

        using var form = MultipartFormHelper.CreateFileForm(
            "Image",
            "product-image.jpg",
            "image/jpeg",
            new byte[] { 1, 2, 3, 4 });

        var response = await _client.PostAsync($"/api/products/{productId}/image", form);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement json = await ReadJsonAsync(response);

        GetString(json, "relatedEntityId").Should().Be(productId);
        GetString(json, "relatedEntityType").Should().Be("product");
        GetString(json, "fileCategory").Should().Be("product-image");
        GetString(json, "originalFileName").Should().Be("product-image.jpg");
        GetString(json, "contentType").Should().Be("image/jpeg");
    }

    [Fact]
    public async Task UploadProductImage_WithPng_ShouldReturnSuccess()
    {
        string productId = await CreateProductAsync();

        using var form = MultipartFormHelper.CreateFileForm(
            "Image",
            "product-image.png",
            "image/png",
            new byte[] { 1, 2, 3, 4 });

        var response = await _client.PostAsync($"/api/products/{productId}/image", form);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement json = await ReadJsonAsync(response);

        GetString(json, "relatedEntityId").Should().Be(productId);
        GetString(json, "originalFileName").Should().Be("product-image.png");
        GetString(json, "contentType").Should().Be("image/png");
    }

    [Fact]
    public async Task UploadProductImage_WithTxtFile_ShouldReturnBadRequest()
    {
        string productId = await CreateProductAsync();

        using var form = MultipartFormHelper.CreateFileForm(
            "Image",
            "product-image.txt",
            "text/plain",
            new byte[] { 1, 2, 3, 4 });

        var response = await _client.PostAsync($"/api/products/{productId}/image", form);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadProductImage_WithOversizedFile_ShouldReturnBadRequest()
    {
        string productId = await CreateProductAsync();

        byte[] oversizedContent = new byte[(2 * 1024 * 1024) + 1];

        using var form = MultipartFormHelper.CreateFileForm(
            "Image",
            "large-product-image.jpg",
            "image/jpeg",
            oversizedContent);

        var response = await _client.PostAsync($"/api/products/{productId}/image", form);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<string> CreateProductAsync()
    {
        string suffix = Guid.NewGuid().ToString("N")[..8];

        var request = new CreateProductTestRequest
        {
            Name = $"Upload Test Product {suffix}",
            SKU = $"UPLOAD-{suffix}",
            Description = "Created for image upload integration test",
            Price = 100,
            QuantityInStock = 10,
            SupplierName = "Upload Test Supplier",
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
