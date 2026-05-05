using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Authorization;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.API.Controllers;

[ApiExplorerSettings(GroupName = "Lookups")]
[ApiController]
[Authorize(Policy = PolicyNames.AdminOrTeacher)]
[Route("api/v1/[controller]")]
public class AcademicController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentTenantContext _tenant;

    public AcademicController(IApplicationDbContext context, ICurrentTenantContext tenant)
    {
        _context = context;
        _tenant = tenant;
    }

    [HttpGet("catalog")]
    public async Task<IActionResult> Catalog(CancellationToken cancellationToken)
    {
        var years = await _context.AcademicYears
            .OrderByDescending(x => x.StartDate)
            .Select(x => new IdNameDto(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        var classes = await _context.Classes
            .OrderBy(x => x.Name)
            .Select(x => new IdNameDto(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        var sections = await _context.Sections
            .OrderBy(x => x.Name)
            .Select(x => new SectionDto(x.Id, x.Name, x.ClassId, x.Capacity))
            .ToListAsync(cancellationToken);

        return Ok(new { academicYears = years, classes, sections });
    }

    [HttpPost("years")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> CreateYear([FromBody] CreateAcademicYearRequest request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant || _tenant.SchoolId is null)
            return BadRequest(new { message = "Tenant is required." });

        var entity = new AcademicYear
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = request.IsActive,
            SchoolId = _tenant.SchoolId.Value
        };

        await _context.AcademicYears.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new { id = entity.Id });
    }

    [HttpPut("years/{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> UpdateYear(Guid id, [FromBody] UpdateAcademicYearRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest(new { message = "Route id and body id must match." });

        var entity = await _context.AcademicYears.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return NotFound();

        entity.Name = request.Name.Trim();
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.IsActive = request.IsActive;
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("years/{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> DeleteYear(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.AcademicYears.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return NotFound();

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("classes")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> CreateClass([FromBody] CreateClassRequest request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant || _tenant.SchoolId is null)
            return BadRequest(new { message = "Tenant is required." });

        var entity = new Classes
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            SchoolId = _tenant.SchoolId.Value
        };

        await _context.Classes.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new { id = entity.Id });
    }

    [HttpPut("classes/{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> UpdateClass(Guid id, [FromBody] UpdateClassRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest(new { message = "Route id and body id must match." });

        var entity = await _context.Classes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return NotFound();

        entity.Name = request.Name.Trim();
        entity.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("classes/{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> DeleteClass(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.Classes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return NotFound();

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("sections")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> CreateSection([FromBody] CreateSectionRequest request, CancellationToken cancellationToken)
    {
        var cls = await _context.Classes.FirstOrDefaultAsync(x => x.Id == request.ClassId, cancellationToken);
        if (cls is null)
            return BadRequest(new { message = "Class not found." });

        var entity = new Section
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Capacity = request.Capacity,
            ClassId = request.ClassId
        };

        await _context.Sections.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new { id = entity.Id });
    }

    [HttpPut("sections/{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> UpdateSection(Guid id, [FromBody] UpdateSectionRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest(new { message = "Route id and body id must match." });

        var entity = await _context.Sections.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return NotFound();

        entity.Name = request.Name.Trim();
        entity.Capacity = request.Capacity;
        entity.ClassId = request.ClassId;
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("sections/{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> DeleteSection(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.Sections.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
            return NotFound();

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    public sealed record IdNameDto(Guid Id, string Name);
    public sealed record SectionDto(Guid Id, string Name, Guid ClassId, int Capacity);
    public sealed record CreateAcademicYearRequest(string Name, DateTime StartDate, DateTime EndDate, bool IsActive);
    public sealed record UpdateAcademicYearRequest(Guid Id, string Name, DateTime StartDate, DateTime EndDate, bool IsActive);
    public sealed record CreateClassRequest(string Name, string? Description);
    public sealed record UpdateClassRequest(Guid Id, string Name, string? Description);
    public sealed record CreateSectionRequest(string Name, int Capacity, Guid ClassId);
    public sealed record UpdateSectionRequest(Guid Id, string Name, int Capacity, Guid ClassId);
}
