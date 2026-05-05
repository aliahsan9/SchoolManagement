using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Authorization;
using SchoolManagement.Application.Features.Lookups.Queries.GetAcademicCatalog;

namespace SchoolManagement.API.Controllers;

[ApiExplorerSettings(GroupName = "Lookups")]
[ApiController]
[Authorize(Policy = PolicyNames.AdminOrTeacher)]
[Route("api/v1/[controller]")]
public class AcademicController : ControllerBase
{
    private readonly IMediator _mediator;

    public AcademicController(IMediator mediator) => _mediator = mediator;

    [HttpGet("catalog")]
    public async Task<IActionResult> Catalog(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAcademicCatalogQuery(), cancellationToken));
}
