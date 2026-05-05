using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Features.Exams.DTOs;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Exams.Queries.GetExams;

public sealed class GetExamsQueryHandler(
    IApplicationDbContext context,
    ICurrentTenantContext tenant)
    : IRequestHandler<GetExamsQuery, List<ExamListDto>>
{
    public async Task<List<ExamListDto>> Handle(GetExamsQuery request, CancellationToken cancellationToken)
    {
        if (!tenant.HasTenant || tenant.SchoolId is null)
            return [];

        var schoolId = tenant.SchoolId.Value;

        return await context.Exams.AsNoTracking()
            .Where(e => e.AcademicYear.SchoolId == schoolId)
            .OrderByDescending(e => e.StartDate)
            .Select(e => new ExamListDto
            {
                Id = e.Id,
                Name = e.Name,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                AcademicYear = e.AcademicYear.Name
            })
            .ToListAsync(cancellationToken);
    }
}
