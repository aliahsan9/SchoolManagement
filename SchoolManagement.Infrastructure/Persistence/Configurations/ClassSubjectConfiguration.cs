using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Persistence.Configurations
{
    public class ClassSubjectConfiguration : IEntityTypeConfiguration<ClassSubject>
    {
        public void Configure(EntityTypeBuilder<ClassSubject> builder)
        {
            builder.ToTable("ClassSubjects");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Classes)
                .WithMany(x => x.ClassSubjects)
                .HasForeignKey(x => x.ClassId);

            builder.HasOne(x => x.Subject)
                .WithMany()
                .HasForeignKey(x => x.SubjectId);
        }
    }
}
