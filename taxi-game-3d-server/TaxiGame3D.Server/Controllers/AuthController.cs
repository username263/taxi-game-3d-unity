using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaxiGame3D.Server.DTOs;
using TaxiGame3D.Server.Repositories;
using TaxiGame3D.Server.Services;

namespace TaxiGame3D.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    readonly UserRepository userRepository;
    readonly TokenService tokenService;

    public AuthController(UserRepository userRepository, TokenService tokenService)
    {
        this.userRepository = userRepository;
        this.tokenService = tokenService;
    }
    
    [HttpPost("LoginEmail")]
    [ProducesResponseType(typeof(LoginResponse), 200)]
    public async Task<ActionResult> LoginWithEmail([FromBody] LoginWithEmailRequest body)
    {
        var user = await userRepository.FindByEmail(body.Email);
        if (user == null)
            return NotFound();

        if (user.Password != body.Password)
            return Forbid();

        var token = tokenService.Generate(user);
        return Ok(new LoginResponse
        {
            Token = token.token,
            ExpireUtc = token.expireUtc
        });
    }

    [HttpPost("CreateEmail")]
    [ProducesResponseType(typeof(LoginResponse), 201)]
    public async Task<ActionResult> CreateWithEmail([FromBody] LoginWithEmailRequest body)
    {
        var user = await userRepository.FindByEmail(body.Email);
        if (user != null)
            return Conflict();

        if (body.Password.Length < 8)
            return Forbid();

        user = new()
        {
            Email = body.Email,
            Password = body.Password
        };
        await userRepository.Create(user);

        var token = tokenService.Generate(user);
        return StatusCode(StatusCodes.Status201Created, new LoginResponse
        {
            Token = token.token,
            ExpireUtc = token.expireUtc
        });
    }

    [HttpGet("RefreshToken")]
    [ProducesResponseType(typeof(LoginResponse), 200)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> RefreshToken()
    {
        var userId = ClaimHelper.FindNameIdentifier(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await userRepository.Get(userId);
        if (user == null)
            return Forbid();

        var token = tokenService.Generate(user);
        return Ok(new LoginResponse
        {
            Token = token.token,
            ExpireUtc = token.expireUtc
        });
    }
}
