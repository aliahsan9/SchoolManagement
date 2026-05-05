using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Features.Billing.DTOs;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Features.Billing.Commands.PaySchoolSubscription;

public sealed class PaySchoolSubscriptionCommandHandler(
    IApplicationDbContext context,
    ICurrentTenantContext tenant,
    IPaymentGateway paymentGateway)
    : IRequestHandler<PaySchoolSubscriptionCommand, SchoolSubscriptionDto?>
{
    public async Task<SchoolSubscriptionDto?> Handle(PaySchoolSubscriptionCommand request, CancellationToken cancellationToken)
    {
        if (!tenant.HasTenant || tenant.SchoolId is null)
            return null;

        var subscription = await context.SchoolSubscriptions
            .Where(s => s.SchoolId == tenant.SchoolId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (subscription is null)
            return null;

        var amount = request.YearlyPremium
            ? subscription.YearlyPriceSnapshot
            : subscription.MonthlyPriceSnapshot;

        var charge = await paymentGateway.ChargeAsync(
            amount,
            "PKR",
            request.PaymentMethod.Trim(),
            $"sub-{subscription.Id}-{Guid.NewGuid():N}",
            cancellationToken);

        if (!charge.Succeeded)
            throw new InvalidOperationException(charge.FailureMessage ?? "Payment was declined.");

        await context.SubscriptionPayments.AddAsync(new SubscriptionPayment
        {
            Id = Guid.NewGuid(),
            SchoolSubscriptionId = subscription.Id,
            Amount = amount,
            Currency = "PKR",
            PaymentMethod = request.PaymentMethod.Trim(),
            ExternalTransactionId = charge.TransactionId,
            PaidAtUtc = DateTime.UtcNow,
            Notes = request.YearlyPremium ? "Yearly premium" : "Monthly standard"
        }, cancellationToken);

        subscription.Status = SubscriptionStatus.Active;
        subscription.Plan = request.YearlyPremium ? SubscriptionPlanType.YearlyPremium : SubscriptionPlanType.MonthlyStandard;
        subscription.CurrentPeriodEndUtc = request.YearlyPremium
            ? DateTime.UtcNow.AddYears(1)
            : DateTime.UtcNow.AddMonths(1);

        await context.SaveChangesAsync(cancellationToken);

        return new SchoolSubscriptionDto
        {
            Id = subscription.Id,
            SchoolId = subscription.SchoolId,
            Plan = subscription.Plan,
            Status = subscription.Status,
            TrialEndsAtUtc = subscription.TrialEndsAtUtc,
            CurrentPeriodEndUtc = subscription.CurrentPeriodEndUtc,
            MonthlyPrice = subscription.MonthlyPriceSnapshot,
            YearlyPrice = subscription.YearlyPriceSnapshot
        };
    }
}
