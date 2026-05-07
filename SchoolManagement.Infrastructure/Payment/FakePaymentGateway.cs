using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Infrastructure.Payment
{
    public sealed class FakePaymentGateway : IPaymentGateway
    {
        public Task<PaymentChargeResult> ChargeAsync(
            decimal amount,
            string currency,
            string paymentMethod,
            string merchantReference,
            CancellationToken cancellationToken = default)
        {
            _ = amount;
            _ = currency;
            _ = paymentMethod;
            _ = merchantReference;
            return Task.FromResult(new PaymentChargeResult(true, $"sim_{Guid.NewGuid():N}", null));
        }
    }
}