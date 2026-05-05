using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Persistence.Configurations
{
    public class SchoolConfiguration : IEntityTypeConfiguration<School>
    {
        public void Configure(EntityTypeBuilder<School> builder)
        {
            builder.ToTable("Schools");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Subdomain)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Subdomain)
                .IsUnique();

            builder.Property(x => x.Email)
                .HasMaxLength(150);

            builder.HasMany(x => x.Branches)
                .WithOne(x => x.School)
                .HasForeignKey(x => x.SchoolId);
        }
    }
}
