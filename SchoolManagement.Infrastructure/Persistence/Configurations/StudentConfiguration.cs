using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Persistence.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.AdmissionNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => new { x.SchoolId, x.AdmissionNumber })
                .IsUnique();

            builder.Property(x => x.Gender)
                .HasMaxLength(10);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.School)
                .WithMany()
                .HasForeignKey(x => x.SchoolId);
        }
    }
}
