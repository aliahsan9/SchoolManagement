using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Authorization;
using SchoolManagement.Application.Features.Fees.Commands.RecordStudentFeePayment;
using SchoolManagement.Application.Features.Fees.Queries.GetStudentFees;

namespace SchoolManagement.API.Controllers;

[ApiExplorerSettings(GroupName = "Fees")]
[ApiController]
[Authorize(Policy = PolicyNames.AdminOrTeacher)]
[Route("api/v1/[controller]")]
public class FeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FeesController(IMediator mediator) => _mediator = mediator;

    [HttpGet("by-student/{studentId:guid}")]
    public async Task<IActionResult> GetByStudent(Guid studentId, CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetStudentFeesQuery(studentId), cancellationToken));

    [HttpPost("payments")]
    [Authorize(Policy = PolicyNames.AdminOnly)]
    public async Task<IActionResult> RecordPayment([FromBody] RecordStudentFeePaymentCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return Ok(new { paymentId = id });
    }
}
