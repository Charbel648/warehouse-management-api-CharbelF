using Warehouse.Application.Common.Auth;

namespace Warehouse.Api.IntegrationTests.Infrastructure;

public class TestCurrentUserService : ICurrentUserService
{
    public string FirebaseUid => "integration-test-user";

    public string? Email => "integration@test.com";

    public string? Role => "admin";
}
