using FluentValidation;

namespace SchoolManagement.Application.Features.Fees.Commands.RecordStudentFeePayment;

public sealed class RecordStudentFeePaymentCommandValidator : AbstractValidator<RecordStudentFeePaymentCommand>
{
    public RecordStudentFeePaymentCommandValidator()
    {
        RuleFor(x => x.StudentFeeId).NotEmpty();
        RuleFor(x => x.AmountPaid).GreaterThan(0);
        RuleFor(x => x.PaymentMethod).NotEmpty().MaximumLength(50);
    }
}
