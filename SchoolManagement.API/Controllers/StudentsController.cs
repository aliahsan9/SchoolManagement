using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Authorization;
using SchoolManagement.Application.Features.Students.Commands.CreateStudent;
using SchoolManagement.Application.Features.Students.Commands.DeleteStudent;
using SchoolManagement.Application.Features.Students.Commands.EnrollStudent;
using SchoolManagement.Application.Features.Students.Commands.LinkStudentParent;
using SchoolManagement.Application.Features.Students.Commands.UpdateStudent;
using SchoolManagement.Application.Features.Students.Queries.GetAllStudents;
using SchoolManagement.Application.Features.Students.Queries.GetStudentById;
using SchoolManagement.Application.Features.Students.Queries.GetStudentByUserId;
using SchoolManagement.Application.Features.Students.Queries.GetStudentsPaged;

namespace SchoolManagement.API.Controllers;

[ApiExplorerSettings(GroupName = "Students")]
[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = PolicyNames.AdminOrTeacher)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAllStudentsQuery(), cancellationToken));

    [HttpGet("paged")]
    [Authorize(Policy = PolicyNames.AdminOrTeacher)]
    public async Task<IActionResult> GetPaged([FromQuery] GetStudentsPagedQuery query, CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(query, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetStudentByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("me")]
    [Authorize(Policy = PolicyNames.StudentOnly)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null || !Guid.TryParse(userId, out var uid))
            return Unauthorized();

        var result = await _mediator.Send(new GetStudentByUserIdQuery(uid), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> Create([FromBody] CreateStudentCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest(new { message = "Route id and body id must match." });

        var ok = await _mediator.Send(command, cancellationToken);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var ok = await _mediator.Send(new DeleteStudentCommand(id), cancellationToken);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/enrollments")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> Enroll(Guid id, [FromBody] EnrollStudentRequest body, CancellationToken cancellationToken)
    {
        var enrollmentId = await _mediator.Send(
            new EnrollStudentCommand(id, body.ClassId, body.SectionId, body.AcademicYearId, body.RollNumber),
            cancellationToken);
        return Ok(new { enrollmentId });
    }

    [HttpPost("{id:guid}/parents")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> LinkParent(Guid id, [FromBody] LinkParentRequest body, CancellationToken cancellationToken)
    {
        await _mediator.Send(new LinkStudentParentCommand(id, body.ParentId, body.Relation), cancellationToken);
        return NoContent();
    }

    public sealed record EnrollStudentRequest(Guid ClassId, Guid SectionId, Guid AcademicYearId, string RollNumber);
    public sealed record LinkParentRequest(Guid ParentId, string Relation);
}
