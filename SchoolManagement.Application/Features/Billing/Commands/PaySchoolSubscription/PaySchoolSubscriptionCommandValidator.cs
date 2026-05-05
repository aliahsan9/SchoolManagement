using FluentValidation;

namespace SchoolManagement.Application.Features.Billing.Commands.PaySchoolSubscription;

public sealed class PaySchoolSubscriptionCommandValidator : AbstractValidator<PaySchoolSubscriptionCommand>
{
    public PaySchoolSubscriptionCommandValidator()
    {
        RuleFor(x => x.PaymentMethod).NotEmpty().MaximumLength(50);
    }
}
