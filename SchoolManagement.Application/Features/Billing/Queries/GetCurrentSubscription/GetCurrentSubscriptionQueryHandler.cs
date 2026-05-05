using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Features.Billing.DTOs;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Billing.Queries.GetCurrentSubscription;

public sealed class GetCurrentSubscriptionQueryHandler(
    IApplicationDbContext context,
    ICurrentTenantContext tenant)
    : IRequestHandler<GetCurrentSubscriptionQuery, SchoolSubscriptionDto?>
{
    public async Task<SchoolSubscriptionDto?> Handle(GetCurrentSubscriptionQuery request, CancellationToken cancellationToken)
    {
        if (!tenant.HasTenant || tenant.SchoolId is null)
            return null;

        return await context.SchoolSubscriptions.AsNoTracking()
            .Where(s => s.SchoolId == tenant.SchoolId)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SchoolSubscriptionDto
            {
                Id = s.Id,
                SchoolId = s.SchoolId,
                Plan = s.Plan,
                Status = s.Status,
                TrialEndsAtUtc = s.TrialEndsAtUtc,
                CurrentPeriodEndUtc = s.CurrentPeriodEndUtc,
                MonthlyPrice = s.MonthlyPriceSnapshot,
                YearlyPrice = s.YearlyPriceSnapshot
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
