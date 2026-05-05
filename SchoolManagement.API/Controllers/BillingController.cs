using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Authorization;
using SchoolManagement.Application.Features.Billing.Commands.PaySchoolSubscription;
using SchoolManagement.Application.Features.Billing.Queries.GetCurrentSubscription;

namespace SchoolManagement.API.Controllers;

[ApiExplorerSettings(GroupName = "Billing")]
[ApiController]
[Authorize(Policy = PolicyNames.AdminOnly)]
[Route("api/v1/[controller]")]
public class BillingController : ControllerBase
{
    private readonly IMediator _mediator;

    public BillingController(IMediator mediator) => _mediator = mediator;

    [HttpGet("subscription")]
    public async Task<IActionResult> GetSubscription(CancellationToken cancellationToken)
    {
        var sub = await _mediator.Send(new GetCurrentSubscriptionQuery(), cancellationToken);
        return sub is null ? NotFound() : Ok(sub);
    }

    [HttpPost("subscription/pay")]
    public async Task<IActionResult> Pay([FromBody] PaySchoolSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result is null ? BadRequest() : Ok(result);
    }
}
