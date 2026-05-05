using MediatR;

namespace SchoolManagement.Application.Features.Fees.Commands.RecordStudentFeePayment;

public sealed record RecordStudentFeePaymentCommand(
    Guid StudentFeeId,
    decimal AmountPaid,
    string PaymentMethod) : IRequest<Guid>;
