using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Persistence.Configurations;

public class SchoolSubscriptionConfiguration : IEntityTypeConfiguration<SchoolSubscription>
{
    public void Configure(EntityTypeBuilder<SchoolSubscription> builder)
    {
        builder.ToTable("SchoolSubscriptions");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.SchoolId);

        builder.HasOne(x => x.School)
            .WithMany(x => x.Subscriptions)
            .HasForeignKey(x => x.SchoolId);

        builder.Property(x => x.MonthlyPriceSnapshot).HasPrecision(18, 2);
        builder.Property(x => x.YearlyPriceSnapshot).HasPrecision(18, 2);
    }
}
