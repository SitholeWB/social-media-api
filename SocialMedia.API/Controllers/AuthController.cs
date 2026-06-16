namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/{tenantId}/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public AuthController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [Authorize]
    [HttpPost("me")]
    public async Task<ActionResult<AuthResponse>> GetUserById(CancellationToken cancellationToken)
    {
        try
        {
            var userId = this.GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { error = "User ID not found in token." });
            }
            var command = new GetUserByIdQuery(userId.Value);
            var response = await _dispatcher.QueryAsync<GetUserByIdQuery, AuthResponse>(command, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, stack = ex.StackTrace });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new LoginCommand(request);
            var response = await _dispatcher.SendAsync<LoginCommand, AuthResponse>(command, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex) when (ex.Message.Contains("Invalid credentials") || ex.Message.Contains("banned"))
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, stack = ex.StackTrace });
        }
    }

    [AllowAnonymous]
    [HttpPost("google")]
    public async Task<ActionResult<AuthResponse>> LoginWithGoogle(GoogleLoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new LoginWithGoogleCommand(request);
            var response = await _dispatcher.SendAsync<LoginWithGoogleCommand, AuthResponse>(command, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, stack = ex.StackTrace });
        }
    }

    /// <summary>
    /// Generic social login endpoint — supports Facebook, X (Twitter), GitHub, and more.
    /// POST body: { "provider": "Facebook" | "Twitter" | "GitHub" | "Google", "accessToken": "..." }
    /// </summary>
    [AllowAnonymous]
    [HttpPost("social")]
    public async Task<ActionResult<AuthResponse>> LoginWithSocial(ExternalLoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new LoginWithExternalProviderCommand(request);
            var response = await _dispatcher.SendAsync<LoginWithExternalProviderCommand, AuthResponse>(command, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }


    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var command = new RegisterCommand(request);
            var response = await _dispatcher.SendAsync<RegisterCommand, AuthResponse>(command, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, stack = ex.StackTrace });
        }
    }

    [Authorize]
    [HttpPost("exchange/{newTenantId}")]
    public async Task<ActionResult<AuthResponse>> ExchangeTenantToken(string newTenantId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = this.GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { error = "User ID not found in token." });
            }
            var command = new ExchangeTenantCommand(userId.Value, newTenantId);
            var response = await _dispatcher.SendAsync<ExchangeTenantCommand, AuthResponse>(command, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var command = new ForgotPasswordCommand(request);
        var result = await _dispatcher.SendAsync<ForgotPasswordCommand, bool>(command, cancellationToken);
        return Ok(result);
    }
}
