namespace SchoolManagement.Application.Interfaces
{
    public interface IPaymentGateway
    {
        Task<PaymentChargeResult> ChargeAsync(
            decimal amount,
            string currency,
            string paymentMethod,
            string merchantReference,
            CancellationToken cancellationToken = default);
    }

    public sealed record PaymentChargeResult(bool Succeeded, string? TransactionId, string? FailureMessage);
}