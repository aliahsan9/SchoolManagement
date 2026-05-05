using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Features.Fees.Commands.RecordStudentFeePayment;

public sealed class RecordStudentFeePaymentCommandHandler(
    IApplicationDbContext context,
    IPaymentGateway paymentGateway,
    ICurrentTenantContext tenant)
    : IRequestHandler<RecordStudentFeePaymentCommand, Guid>
{
    public async Task<Guid> Handle(RecordStudentFeePaymentCommand request, CancellationToken cancellationToken)
    {
        if (!tenant.HasTenant || tenant.SchoolId is null)
            throw new InvalidOperationException("Tenant is required.");

        var fee = await context.StudentFees
            .Include(f => f.Student)
            .FirstOrDefaultAsync(f => f.Id == request.StudentFeeId, cancellationToken)
            ?? throw new InvalidOperationException("Student fee not found.");

        if (fee.Student.SchoolId != tenant.SchoolId.Value)
            throw new UnauthorizedAccessException("Fee does not belong to this school.");

        var charge = await paymentGateway.ChargeAsync(
            request.AmountPaid,
            "PKR",
            request.PaymentMethod.Trim(),
            $"fee-{fee.Id}-{Guid.NewGuid():N}",
            cancellationToken);

        if (!charge.Succeeded)
            throw new InvalidOperationException(charge.FailureMessage ?? "Payment failed.");

        var existingPaid = await context.Payments
            .Where(p => p.StudentFeeId == fee.Id)
            .SumAsync(p => p.AmountPaid, cancellationToken);

        var due = await context.FeeStructures
            .Where(fs => fs.Id == fee.FeeStructureId)
            .Select(fs => fs.Amount)
            .FirstAsync(cancellationToken);

        var totalAfter = existingPaid + request.AmountPaid;

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            StudentFeeId = fee.Id,
            AmountPaid = request.AmountPaid,
            PaymentDate = DateTime.UtcNow,
            PaymentMethod = request.PaymentMethod.Trim(),
            TransactionId = charge.TransactionId
        };

        await context.Payments.AddAsync(payment, cancellationToken);
        fee.Status = totalAfter >= due ? "Paid" : "Partial";

        await context.SaveChangesAsync(cancellationToken);
        return payment.Id;
    }
}
