using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Constants;
using SchoolManagement.Infrastructure.MultiTenancy;
using SchoolManagement.Infrastructure.Persistence;

namespace SchoolManagement.API.Middleware;

public sealed class TenantResolutionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, SchoolDbContext db)
    {
        var previous = TenantScope.SchoolId;
        try
        {
            Guid? schoolId = null;

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

                if (!string.IsNullOrWhiteSpace(sub))
                {
                    var key = sub.Trim().ToLowerInvariant();
                    schoolId = await db.Schools.AsNoTracking()
                        .Where(s => s.Subdomain == key && s.IsActive)
                        .Select(s => (Guid?)s.Id)
                        .FirstOrDefaultAsync(context.RequestAborted);
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
