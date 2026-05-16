using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using SocialMedia.Infrastructure;
using SocialMedia.Domain;
using System;

namespace SocialMedia.API.Middleware;

public class TenantValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantValidationMiddleware> _logger;

    public TenantValidationMiddleware(RequestDelegate next, ILogger<TenantValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. If the user is authenticated, verify their token's tenant matches the route's tenant
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var routeTenantIdValue = context.Request.RouteValues["tenantId"]?.ToString();
            var claimTenantIdValue = context.User.FindFirst("tenantId")?.Value;

            // Only validate if both are present
            if (!string.IsNullOrEmpty(routeTenantIdValue) && !string.IsNullOrEmpty(claimTenantIdValue))
            {
                var isSuperAdmin = context.User.IsInRole(UserRole.SuperAdmin.ToString());
                var isSuperTenant = claimTenantIdValue.Equals(Tenant.SuperTenantId.ToString(), StringComparison.OrdinalIgnoreCase);

                if (routeTenantIdValue != claimTenantIdValue && !(isSuperAdmin && isSuperTenant))
                {
                    _logger.LogWarning("Tenant mismatch: Route {RouteTenantId} != Claim {ClaimTenantId}", routeTenantIdValue, claimTenantIdValue);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden: Tenant mismatch.");
                    return;
                }
            }
        }

        await _next(context);
    }
}
