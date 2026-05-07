using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Persistence.Configurations
{
    public class SubscriptionPaymentConfiguration : IEntityTypeConfiguration<SubscriptionPayment>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPayment> builder)
        {
            builder.ToTable("SubscriptionPayments");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.SchoolSubscriptionId);
            builder.HasIndex(x => x.ExternalTransactionId);

            builder.Property(x => x.Amount).HasPrecision(18, 2);

            builder.HasOne(x => x.SchoolSubscription)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.SchoolSubscriptionId);
        }
    }
}