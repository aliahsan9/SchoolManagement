using MediatR;
using SchoolManagement.Application.Features.Billing.DTOs;

namespace SchoolManagement.Application.Features.Billing.Queries.GetCurrentSubscription;

public sealed record GetCurrentSubscriptionQuery : IRequest<SchoolSubscriptionDto?>;
