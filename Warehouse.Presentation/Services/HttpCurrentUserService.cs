using System.Security.Claims;
using Warehouse.Application.Common.Auth;

namespace Warehouse.Presentation.Services;

public class HttpCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string FirebaseUid
    {
        get
        {
            string? firebaseUid =
                _httpContextAccessor.HttpContext?.User.FindFirstValue("firebase_uid")
                ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("user_id")
                ?? _httpContextAccessor.HttpContext?.User.FindFirstValue("sub");

            return firebaseUid ?? "unknown";
        }
    }

    public string? Email =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue("email");

    public string? Role =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue("role");
}

