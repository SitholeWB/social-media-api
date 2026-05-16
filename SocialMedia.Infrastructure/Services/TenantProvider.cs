using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using SocialMedia.Domain;

namespace SocialMedia.Infrastructure;

public interface ITenantProvider
{
    Guid GetTenantId();
}

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetTenantId()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            return Guid.Empty;

        var routeTenantId = Guid.Empty;
        if (context.Request.RouteValues.TryGetValue("tenantId", out var routeValue) && routeValue != null)
        {
            Guid.TryParse(routeValue.ToString(), out routeTenantId);
        }

        var tenantClaimValue = context.User?.FindFirst("tenantId")?.Value;
        var claimTenantId = Guid.Empty;
        Guid.TryParse(tenantClaimValue, out claimTenantId);

        var isSuperAdmin = context.User?.IsInRole(UserRole.SuperAdmin.ToString()) ?? false;
        var isSuperTenant = claimTenantId == Tenant.SuperTenantId;

        // If SuperAdmin is accessing another tenant's route, use the route's tenantId
        if (isSuperAdmin && isSuperTenant && routeTenantId != Guid.Empty)
        {
            return routeTenantId;
        }

        // Otherwise, use the claim tenantId (standard multi-tenancy enforcement)
        if (claimTenantId != Guid.Empty)
        {
            return claimTenantId;
        }

        return routeTenantId != Guid.Empty ? routeTenantId : Guid.Empty;
    }
}
