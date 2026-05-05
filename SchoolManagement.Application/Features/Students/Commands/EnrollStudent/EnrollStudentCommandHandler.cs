using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Features.Students.Commands.EnrollStudent;

public sealed class EnrollStudentCommandHandler(IApplicationDbContext context)
    : IRequestHandler<EnrollStudentCommand, Guid>
{
    public async Task<Guid> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.StudentEnrollments.AnyAsync(
            e => e.StudentId == request.StudentId &&
                 e.AcademicYearId == request.AcademicYearId &&
                 e.ClassId == request.ClassId,
            cancellationToken);

        if (exists)
            throw new InvalidOperationException("Student is already enrolled for this class and academic year.");

        var enrollment = new StudentEnrollment
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            ClassId = request.ClassId,
            SectionId = request.SectionId,
            AcademicYearId = request.AcademicYearId,
            RollNumber = request.RollNumber.Trim()
        };

        await context.StudentEnrollments.AddAsync(enrollment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return enrollment.Id;
    }
}
