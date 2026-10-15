using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.Ports.In;

namespace UserManagement.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(ILoginUseCase loginUseCase) : ControllerBase
{
    [HttpPost("token")]
    [AllowAnonymous]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponse>> CreateToken(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await loginUseCase.ExecuteAsync(request, cancellationToken);
        return Ok(response);
    }
}