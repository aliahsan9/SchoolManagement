using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Infrastructure.MultiTenancy;
using SchoolManagement.Infrastructure.Persistence;

namespace SchoolManagement.API.Middleware;

public sealed class SubscriptionEnforcementMiddleware(RequestDelegate next)
{
    private static readonly HashSet<string> ExemptPathFragments =
    [
        "/swagger",
        "/api/v1/auth",
        "/health",
        "/favicon"
    ];

    public async Task InvokeAsync(HttpContext context, SchoolDbContext db)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (ExemptPathFragments.Any(p => path.Contains(p, StringComparison.OrdinalIgnoreCase)))
        {
            await next(context);
            return;
        }

        if (TenantScope.SchoolId is not Guid schoolId)
        {
            await next(context);
            return;
        }

        var subscription = await db.SchoolSubscriptions.AsNoTracking()
            .Where(s => s.SchoolId == schoolId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(context.RequestAborted);

        if (subscription is null)
        {
            context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "No subscription found for this school. Complete billing setup.",
                code = "SUBSCRIPTION_MISSING"
            }, context.RequestAborted);
            return;
        }

        var now = DateTime.UtcNow;
        var billing = path.Contains("/api/v1/billing", StringComparison.OrdinalIgnoreCase);

        var allowed = subscription.Status switch
        {
            SubscriptionStatus.Trialing => now < subscription.TrialEndsAtUtc,
            SubscriptionStatus.Active => subscription.CurrentPeriodEndUtc is null || now < subscription.CurrentPeriodEndUtc.Value,
            SubscriptionStatus.PastDue => billing,
            SubscriptionStatus.Cancelled => billing,
            _ => false
        };

        if (!allowed)
        {
            context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Subscription is not active. Renew or upgrade your plan.",
                code = "SUBSCRIPTION_INACTIVE",
                status = subscription.Status.ToString()
            }, context.RequestAborted);
            return;
        }

        await next(context);
    }
}
