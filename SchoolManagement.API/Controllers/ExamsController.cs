using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Authorization;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.API.Controllers;

[ApiExplorerSettings(GroupName = "Exams")]
[ApiController]
[Authorize(Policy = PolicyNames.AdminOrTeacher)]
[Route("api/v1/[controller]")]
public class ExamsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentTenantContext _tenant;

    public ExamsController(IApplicationDbContext context, ICurrentTenantContext tenant)
    {
        _context = context;
        _tenant = tenant;
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var data = await _context.Exams
            .Include(e => e.AcademicYear)
            .OrderByDescending(e => e.StartDate)
            .Select(e => new ExamResponse(e.Id, e.Name, e.StartDate, e.EndDate, e.AcademicYearId, e.AcademicYear.Name))
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var exam = await _context.Exams
            .Include(e => e.AcademicYear)
            .Where(e => e.Id == id)
            .Select(e => new ExamResponse(e.Id, e.Name, e.StartDate, e.EndDate, e.AcademicYearId, e.AcademicYear.Name))
            .FirstOrDefaultAsync(cancellationToken);

        return exam is null ? NotFound() : Ok(exam);
    }

    [HttpPost]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> Create([FromBody] CreateExamRequest request, CancellationToken cancellationToken)
    {
        var year = await _context.AcademicYears.FirstOrDefaultAsync(y => y.Id == request.AcademicYearId, cancellationToken);
        if (year is null)
            return BadRequest(new { message = "Academic year not found." });

        if (_tenant.HasTenant && _tenant.SchoolId is Guid tid && year.SchoolId != tid)
            return Forbid();

        var exam = new Exam
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            AcademicYearId = request.AcademicYearId,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        await _context.Exams.AddAsync(exam, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new { id = exam.Id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExamRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest(new { message = "Route id and body id must match." });

        var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (exam is null)
            return NotFound();

        var year = await _context.AcademicYears.FirstOrDefaultAsync(y => y.Id == request.AcademicYearId, cancellationToken);
        if (year is null)
            return BadRequest(new { message = "Academic year not found." });

        exam.Name = request.Name.Trim();
        exam.AcademicYearId = request.AcademicYearId;
        exam.StartDate = request.StartDate;
        exam.EndDate = request.EndDate;
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (exam is null)
            return NotFound();

        exam.IsDeleted = true;
        exam.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    public sealed record CreateExamRequest(string Name, Guid AcademicYearId, DateTime StartDate, DateTime EndDate);
    public sealed record UpdateExamRequest(Guid Id, string Name, Guid AcademicYearId, DateTime StartDate, DateTime EndDate);
    public sealed record ExamResponse(Guid Id, string Name, DateTime StartDate, DateTime EndDate, Guid AcademicYearId, string AcademicYear);
}
