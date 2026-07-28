using System.Net;
using System.Text.Json;
using FluentAssertions;
using Warehouse.Api.IntegrationTests.Helpers;
using Warehouse.Api.IntegrationTests.Infrastructure;

namespace Warehouse.Api.IntegrationTests.Suppliers;

public class SupplierEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SupplierEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSuppliers_ShouldReturnSuccess()
    {
        var response = await _client.GetAsync("/api/suppliers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string json = await response.Content.ReadAsStringAsync();
        json.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetSupplier_WithExistingId_ShouldReturnSupplier()
    {
        string supplierId = await CreateSupplierAsync();

        var response = await _client.GetAsync($"/api/suppliers/{supplierId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement json = await ReadJsonAsync(response);
        GetString(json, "id").Should().Be(supplierId);
    }

    [Fact]
    public async Task GetSupplier_WithMissingId_ShouldReturnNotFound()
    {
        string missingId = Guid.NewGuid().ToString();

        var response = await _client.GetAsync($"/api/suppliers/{missingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSupplier_WithInvalidId_ShouldReturnBadRequest()
    {
        var response = await _client.GetAsync("/api/suppliers/not-a-guid");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSupplier_WithValidRequest_ShouldReturnCreated()
    {
        var request = CreateSupplierRequest();

        var response = await _client.PostAsync(
            "/api/suppliers",
            JsonContentHelper.Create(request));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        JsonElement json = await ReadJsonAsync(response);

        GetString(json, "id").Should().NotBeNullOrWhiteSpace();
        GetString(json, "name").Should().Be(request.Name);
        GetString(json, "country").Should().Be(request.Country);
        GetString(json, "contactEmail").Should().Be(request.ContactEmail);
        GetBool(json, "isActive").Should().BeTrue();
    }

    [Fact]
    public async Task CreateSupplier_WithDuplicateEmail_ShouldReturnConflict()
    {
        string email = $"supplier-{Guid.NewGuid():N}@test.com";

        var firstRequest = CreateSupplierRequest(email);
        var secondRequest = CreateSupplierRequest(email);

        var firstResponse = await _client.PostAsync(
            "/api/suppliers",
            JsonContentHelper.Create(firstRequest));

        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var secondResponse = await _client.PostAsync(
            "/api/suppliers",
            JsonContentHelper.Create(secondRequest));

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task DeleteSupplier_ShouldDeactivateSupplier()
    {
        string supplierId = await CreateSupplierAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/suppliers/{supplierId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement deleteJson = await ReadJsonAsync(deleteResponse);
        GetBool(deleteJson, "isActive").Should().BeFalse();

        var getResponse = await _client.GetAsync($"/api/suppliers/{supplierId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        JsonElement getJson = await ReadJsonAsync(getResponse);
        GetBool(getJson, "isActive").Should().BeFalse();
    }

    private async Task<string> CreateSupplierAsync(string? email = null)
    {
        var request = CreateSupplierRequest(email);

        var response = await _client.PostAsync(
            "/api/suppliers",
            JsonContentHelper.Create(request));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        JsonElement json = await ReadJsonAsync(response);

        return GetString(json, "id")!;
    }

    private static CreateSupplierTestRequest CreateSupplierRequest(string? email = null)
    {
        string suffix = Guid.NewGuid().ToString("N")[..8];

        return new CreateSupplierTestRequest
        {
            Name = $"Integration Supplier {suffix}",
            Country = "Lebanon",
            ContactEmail = email ?? $"supplier-{suffix}@test.com",
            PhoneNumber = "+96100000000"
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
}
