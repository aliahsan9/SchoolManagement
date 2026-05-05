using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Features.Lookups.DTOs;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.Features.Lookups.Queries.GetAcademicCatalog;

public sealed class GetAcademicCatalogQueryHandler(
    IApplicationDbContext context,
    ICurrentTenantContext tenant)
    : IRequestHandler<GetAcademicCatalogQuery, AcademicCatalogDto>
{
    public async Task<AcademicCatalogDto> Handle(GetAcademicCatalogQuery request, CancellationToken cancellationToken)
    {
        if (!tenant.HasTenant || tenant.SchoolId is null)
            return new AcademicCatalogDto();

        var schoolId = tenant.SchoolId.Value;

        var years = await context.AcademicYears.AsNoTracking()
            .Where(y => y.SchoolId == schoolId)
            .OrderByDescending(y => y.StartDate)
            .Select(y => new IdNameItemDto { Id = y.Id, Name = y.Name })
            .ToListAsync(cancellationToken);

        var classes = await context.Classes.AsNoTracking()
            .Where(c => c.SchoolId == schoolId)
            .OrderBy(c => c.Name)
            .Select(c => new IdNameItemDto { Id = c.Id, Name = c.Name })
            .ToListAsync(cancellationToken);

        var classIds = classes.Select(c => c.Id).ToList();
        var sections = await context.Sections.AsNoTracking()
            .Where(s => classIds.Contains(s.ClassId))
            .OrderBy(s => s.Name)
            .Select(s => new SectionItemDto { Id = s.Id, Name = s.Name, ClassId = s.ClassId })
            .ToListAsync(cancellationToken);

        return new AcademicCatalogDto
        {
            AcademicYears = years,
            Classes = classes,
            Sections = sections
        };
    }
}
