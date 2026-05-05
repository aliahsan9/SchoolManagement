using MediatR;
using SchoolManagement.Application.Features.Billing.DTOs;

namespace SchoolManagement.Application.Features.Billing.Commands.PaySchoolSubscription;

public sealed record PaySchoolSubscriptionCommand(
    bool YearlyPremium,
    string PaymentMethod) : IRequest<SchoolSubscriptionDto?>;
