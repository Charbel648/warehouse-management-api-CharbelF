using System.Net;
using FluentAssertions;
using Warehouse.Api.IntegrationTests.Infrastructure;

namespace Warehouse.Api.IntegrationTests.Swagger;

public class SwaggerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SwaggerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSwaggerJson_ShouldReturnSuccess()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string json = await response.Content.ReadAsStringAsync();

        json.Should().NotBeNullOrWhiteSpace();
        json.Should().Contain("openapi");
    }
}
