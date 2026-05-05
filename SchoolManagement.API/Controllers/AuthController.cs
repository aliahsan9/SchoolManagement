using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Features.Auth.Commands.Login;
using SchoolManagement.Application.Features.Auth.Commands.RefreshToken;
using SchoolManagement.Application.Features.Auth.Commands.Register;

namespace SchoolManagement.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Authentication")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken) =>
            Ok(await _mediator.Send(command, cancellationToken));

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken) =>
            Ok(await _mediator.Send(command, cancellationToken));

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken) =>
            Ok(await _mediator.Send(command, cancellationToken));
    }
}