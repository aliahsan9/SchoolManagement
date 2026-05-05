using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Authorization;
using SchoolManagement.Application.Features.Parents.Commands.CreateParent;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.API.Controllers;

[ApiExplorerSettings(GroupName = "Parents")]
[ApiController]
[Authorize(Policy = PolicyNames.AdminOnly)]
[Route("api/v1/[controller]")]
public class ParentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public ParentsController(IMediator mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var data = await _context.Parents
            .Include(p => p.User)
            .OrderBy(p => p.User.FirstName)
            .ThenBy(p => p.User.LastName)
            .Select(p => new ParentResponse(
                p.Id,
                $"{p.User.FirstName} {p.User.LastName}",
                p.User.Email,
                p.User.PhoneNumber,
                p.Occupation))
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var parent = await _context.Parents
            .Include(p => p.User)
            .Where(p => p.Id == id)
            .Select(p => new ParentResponse(
                p.Id,
                $"{p.User.FirstName} {p.User.LastName}",
                p.User.Email,
                p.User.PhoneNumber,
                p.Occupation))
            .FirstOrDefaultAsync(cancellationToken);

        return parent is null ? NotFound() : Ok(parent);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateParentCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateParentRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest(new { message = "Route id and body id must match." });

        var parent = await _context.Parents.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (parent is null)
            return NotFound();

        parent.User.FirstName = request.FirstName.Trim();
        parent.User.LastName = request.LastName.Trim();
        parent.User.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? string.Empty : request.PhoneNumber.Trim();
        parent.Occupation = string.IsNullOrWhiteSpace(request.Occupation) ? null : request.Occupation.Trim();

        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var parent = await _context.Parents.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (parent is null)
            return NotFound();

        parent.IsDeleted = true;
        parent.DeletedAt = DateTime.UtcNow;
        parent.User.IsDeleted = true;
        parent.User.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    public sealed record UpdateParentRequest(Guid Id, string FirstName, string LastName, string? PhoneNumber, string? Occupation);
    public sealed record ParentResponse(Guid Id, string FullName, string Email, string PhoneNumber, string? Occupation);
}
