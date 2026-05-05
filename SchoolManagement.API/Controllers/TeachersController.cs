using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Authorization;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Constants;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.API.Controllers;

[ApiExplorerSettings(GroupName = "Teachers")]
[ApiController]
[Authorize(Policy = PolicyNames.AdminOrTeacher)]
[Route("api/v1/[controller]")]
public class TeachersController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentTenantContext _tenant;
    private readonly IPasswordHasherService _passwordHasher;

    public TeachersController(IApplicationDbContext context, ICurrentTenantContext tenant, IPasswordHasherService passwordHasher)
    {
        _context = context;
        _tenant = tenant;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var data = await _context.Teachers
            .Include(t => t.User)
            .OrderBy(t => t.User.FirstName)
            .ThenBy(t => t.User.LastName)
            .Select(t => new TeacherResponse(
                t.Id,
                $"{t.User.FirstName} {t.User.LastName}",
                t.User.Email,
                t.EmployeeId,
                t.JoiningDate,
                t.Qualification,
                t.ExperienceYears))
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var teacher = await _context.Teachers
            .Include(t => t.User)
            .Where(t => t.Id == id)
            .Select(t => new TeacherResponse(
                t.Id,
                $"{t.User.FirstName} {t.User.LastName}",
                t.User.Email,
                t.EmployeeId,
                t.JoiningDate,
                t.Qualification,
                t.ExperienceYears))
            .FirstOrDefaultAsync(cancellationToken);

        return teacher is null ? NotFound() : Ok(teacher);
    }

    [HttpPost]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> Create([FromBody] CreateTeacherRequest request, CancellationToken cancellationToken)
    {
        if (!_tenant.HasTenant || _tenant.SchoolId is null)
            return BadRequest(new { message = "Tenant is required." });

        var teacherRole = await _context.Roles.FirstAsync(r => r.Name == RoleNames.Teacher, cancellationToken);
        var user = new User
        {
            Id = Guid.NewGuid(),
            SchoolId = _tenant.SchoolId.Value,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? string.Empty : request.PhoneNumber.Trim(),
            PasswordHash = string.Empty,
            IsActive = true
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.InitialPassword);

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.UserRoles.AddAsync(new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = teacherRole.Id
        }, cancellationToken);

        var teacher = new Teacher
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            SchoolId = _tenant.SchoolId.Value,
            EmployeeId = request.EmployeeId.Trim(),
            JoiningDate = request.JoiningDate,
            Qualification = request.Qualification.Trim(),
            ExperienceYears = request.ExperienceYears
        };

        await _context.Teachers.AddAsync(teacher, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new { id = teacher.Id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeacherRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest(new { message = "Route id and body id must match." });

        var teacher = await _context.Teachers.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (teacher is null)
            return NotFound();

        teacher.User.FirstName = request.FirstName.Trim();
        teacher.User.LastName = request.LastName.Trim();
        teacher.User.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? string.Empty : request.PhoneNumber.Trim();
        teacher.EmployeeId = request.EmployeeId.Trim();
        teacher.JoiningDate = request.JoiningDate;
        teacher.Qualification = request.Qualification.Trim();
        teacher.ExperienceYears = request.ExperienceYears;

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
            teacher.User.PasswordHash = _passwordHasher.HashPassword(teacher.User, request.NewPassword);

        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var teacher = await _context.Teachers.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (teacher is null)
            return NotFound();

        teacher.IsDeleted = true;
        teacher.DeletedAt = DateTime.UtcNow;
        teacher.User.IsDeleted = true;
        teacher.User.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    public sealed record CreateTeacherRequest(
        string FirstName,
        string LastName,
        string Email,
        string? PhoneNumber,
        string InitialPassword,
        string EmployeeId,
        DateTime JoiningDate,
        string Qualification,
        int ExperienceYears);

    public sealed record UpdateTeacherRequest(
        Guid Id,
        string FirstName,
        string LastName,
        string? PhoneNumber,
        string EmployeeId,
        DateTime JoiningDate,
        string Qualification,
        int ExperienceYears,
        string? NewPassword);

    public sealed record TeacherResponse(
        Guid Id,
        string FullName,
        string Email,
        string EmployeeId,
        DateTime JoiningDate,
        string Qualification,
        int ExperienceYears);
}
