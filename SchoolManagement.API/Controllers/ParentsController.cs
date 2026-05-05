using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Authorization;
using SchoolManagement.Application.Features.Parents.Commands.CreateParent;

namespace SchoolManagement.API.Controllers;

[ApiExplorerSettings(GroupName = "Parents")]
[ApiController]
[Authorize(Policy = PolicyNames.AdminOnly)]
[Route("api/v1/[controller]")]
public class ParentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ParentsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateParentCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(new { id });
    }
}
