using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Constants;
using SchoolManagement.Infrastructure.MultiTenancy;
using SchoolManagement.Infrastructure.Persistence;

namespace SchoolManagement.API.Middleware;

public sealed class TenantResolutionMiddleware(RequestDelegate next, IWebHostEnvironment environment)
{
    private static readonly string[] ExemptPathFragments =
    [
        "/swagger",
        "/health",
        "/favicon"
    ];

    public async Task InvokeAsync(HttpContext context, SchoolDbContext db)
    {
        var previous = TenantScope.SchoolId;
        try
        {
            Guid? schoolId = null;
            var path = context.Request.Path.Value ?? string.Empty;

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var claim = context.User.FindFirstValue(ClaimNames.SchoolId);
                if (Guid.TryParse(claim, out var fromJwt))
                    schoolId = fromJwt;
            }
            else
            {
                var sub = context.Request.Headers["X-Tenant-Subdomain"].FirstOrDefault();
                if (string.IsNullOrWhiteSpace(sub))
                    sub = TrySubdomainFromHost(context.Request.Host);

                if (string.IsNullOrWhiteSpace(sub)
                    && environment.IsDevelopment()
                    && context.Request.Host.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                {
                    sub = "demo";
                }

                var isApiRequest = path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase);
                var isExempt = ExemptPathFragments.Any(p => path.Contains(p, StringComparison.OrdinalIgnoreCase));
                if (isApiRequest && isExempt is false && string.IsNullOrWhiteSpace(sub))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = "Tenant header is required. Send X-Tenant-Subdomain for API requests.",
                        code = "TENANT_HEADER_REQUIRED"
                    }, context.RequestAborted);
                    return;
                }

                if (string.IsNullOrWhiteSpace(sub) is false)
                {
                    var key = sub.Trim().ToLowerInvariant();
                    schoolId = await db.Schools.AsNoTracking()
                        .Where(s => s.Subdomain == key && s.IsActive)
                        .Select(s => (Guid?)s.Id)
                        .FirstOrDefaultAsync(context.RequestAborted);

                    if (schoolId is null && isApiRequest && isExempt is false)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            message = $"No active school exists for subdomain '{key}'.",
                            code = "TENANT_NOT_FOUND"
                        }, context.RequestAborted);
                        return;
                    }
                }
            }

            TenantScope.SchoolId = schoolId;
            await next(context);
        }
        finally
        {
            TenantScope.SchoolId = previous;
        }
    }

    private static string? TrySubdomainFromHost(HostString host)
    {
        var hostName = host.Host;
        if (string.IsNullOrEmpty(hostName) || hostName.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            return null;

        var parts = hostName.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
            return null;

        if (parts[0].Equals("www", StringComparison.OrdinalIgnoreCase) && parts.Length >= 3)
            return parts[1];

        return parts[0];
    }
}
