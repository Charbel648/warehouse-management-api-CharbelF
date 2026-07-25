namespace Warehouse.Application.Common.Auth;

public interface ICurrentUserService
{
    string FirebaseUid { get; }

    string? Email { get; }

    string? Role { get; }
}
