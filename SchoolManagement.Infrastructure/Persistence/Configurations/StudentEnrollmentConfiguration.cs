using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Persistence.Configurations
{
    public class StudentEnrollmentConfiguration : IEntityTypeConfiguration<StudentEnrollment>
    {
        public void Configure(EntityTypeBuilder<StudentEnrollment> builder)
        {
            builder.ToTable("StudentEnrollments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RollNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(x => x.Student)
                .WithMany(x => x.Enrollments)
                .HasForeignKey(x => x.StudentId);

            builder.HasOne(x => x.Classes)
                .WithMany()
                .HasForeignKey(x => x.ClassId);

            builder.HasOne(x => x.Section)
                .WithMany()
                .HasForeignKey(x => x.SectionId);

            builder.HasOne(x => x.AcademicYear)
                .WithMany()
                .HasForeignKey(x => x.AcademicYearId);
        }
    }
}
