using Google.Apis.Auth;

namespace SocialMedia.Infrastructure;

public class IdentityService : IIdentityService
{
    private readonly SocialMediaDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IUserActivityRepository _userActivityRepository;
    private readonly IEnumerable<ISocialTokenVerifier> _socialVerifiers;

    public IdentityService(
        SocialMediaDbContext context,
        IConfiguration configuration,
        IUserActivityRepository userActivityRepository,
        IEnumerable<ISocialTokenVerifier> socialVerifiers)
    {
        _context = context;
        _configuration = configuration;
        _userActivityRepository = userActivityRepository;
        _socialVerifiers = socialVerifiers;
    }

    public async Task<AuthResponse> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new Exception("User Not Found");
        }
        var token = GenerateJwtToken(user);
        return new AuthResponse(user.Id.ToString(), user.GetFullName(), user.Email, user.Names, user.Surname, token, user.TenantId.ToString());
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower()
                                                              || u.Email.ToLower() == username.ToLower(), cancellationToken);

        if (user == null || !VerifyPassword(request.Password, user.PasswordHash, user.Id))
        {
            throw new Exception("Invalid credentials");
        }

        if (user.IsBanned)
        {
            throw new Exception("User is banned");
        }

        user.LastActiveAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        var token = GenerateJwtToken(user);
        await _userActivityRepository.RefreshCacheAsync(user.Id, cancellationToken);
        return new AuthResponse(user.Id.ToString(), user.GetFullName(), user.Email, user.Names, user.Surname, token, user.TenantId.ToString());
    }

    public async Task<AuthResponse> LoginWithGoogleAsync(GoogleLoginRequest request, CancellationToken cancellationToken = default)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
        }
        catch (InvalidJwtException)
        {
            throw new Exception("Invalid Google token");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == payload.Email.ToLower(), cancellationToken);

        if (user == null)
        {
            var userId = Guid.NewGuid();
            user = new User
            {
                Username = payload.Email.Split('@')[0], // Use email prefix as username
                Email = payload.Email,
                PasswordHash = HashPassword(Guid.NewGuid().ToString(), userId), // Random password for Google users
                CreatedAt = DateTime.UtcNow,
                Role = UserRole.User,
                Surname = payload.FamilyName,
                Names = payload.GivenName
            };

            // Ensure username is unique
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == user.Username.ToLower(), cancellationToken))
            {
                user.Username = $"{user.Username}_{Guid.NewGuid().ToString().Substring(0, 4)}";
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            var userActivity = await _userActivityRepository.GetByUserIdAsync(userId, true, cancellationToken);
            if (userActivity == default)
            {
                userActivity = new UserActivity { UserId = userId };
                await _userActivityRepository.AddAsync(userActivity, cancellationToken);
            }
        }

        if (user.IsBanned)
        {
            throw new Exception("User is banned");
        }

        user.LastActiveAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        var token = GenerateJwtToken(user);

        await _userActivityRepository.RefreshCacheAsync(user.Id, cancellationToken);
        return new AuthResponse(user.Id.ToString(), user.GetFullName(), user.Email, user.Names, user.Surname, token, user.TenantId.ToString());
    }

    public async Task<AuthResponse> LoginWithExternalProviderAsync(ExternalLoginRequest request, CancellationToken cancellationToken = default)
    {
        // Dispatch to the correct verifier for this provider
        var verifier = _socialVerifiers.FirstOrDefault(v => v.Provider == request.Provider)
            ?? throw new InvalidOperationException($"No token verifier registered for provider '{request.Provider}'. " +
                "Add the provider config in appsettings.json and register the verifier in InfrastructureServiceRegistration.");

        ExternalUserInfo userInfo;
        try
        {
            userInfo = await verifier.VerifyAsync(request.AccessToken, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception($"Social login failed ({request.Provider}): {ex.Message}");
        }

        // Find existing user by email OR create a new one (upsert)
        var user = await _context.Users.FirstOrDefaultAsync(
            u => u.Email.ToLower() == userInfo.Email.ToLower(), cancellationToken);

        if (user == null)
        {
            var userId = Guid.NewGuid();
            var baseUsername = (userInfo.Email.Contains('@')
                ? userInfo.Email.Split('@')[0]
                : userInfo.Email).ToLower();

            user = new User
            {
                Id = userId,
                Username = baseUsername,
                Email = userInfo.Email,
                PasswordHash = HashPassword(Guid.NewGuid().ToString(), userId), // Random — social users don't use passwords
                CreatedAt = DateTime.UtcNow,
                LastActiveAt = DateTime.UtcNow,
                Role = UserRole.User,
                Names = userInfo.GivenName ?? baseUsername,
                Surname = userInfo.FamilyName ?? string.Empty
            };

            // Guarantee unique username
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == user.Username.ToLower(), cancellationToken))
                user.Username = $"{user.Username}_{Guid.NewGuid().ToString()[..4]}";

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            var userActivity = new UserActivity { UserId = userId };
            await _userActivityRepository.AddAsync(userActivity, cancellationToken);
        }

        if (user.IsBanned)
            throw new Exception("User is banned");

        user.LastActiveAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        var token = GenerateJwtToken(user);
        await _userActivityRepository.RefreshCacheAsync(user.Id, cancellationToken);
        return new AuthResponse(user.Id.ToString(), user.GetFullName(), user.Email, user.Names, user.Surname, token, user.TenantId.ToString());
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();
        if (await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()
                                            || u.Email.ToLower() == username.ToLower(), cancellationToken))
        {
            throw new Exception("Username already exists");
        }
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = request.Username.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = HashPassword(request.Password, userId),
            CreatedAt = DateTime.UtcNow,
            Role = UserRole.User, // Explicitly set default
            LastActiveAt = DateTime.UtcNow,
            Names = request.Names,
            Surname = request.Surname
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        var token = GenerateJwtToken(user);
        var userActivity = new UserActivity { UserId = userId };
        await _userActivityRepository.AddAsync(userActivity, cancellationToken);

        return new AuthResponse(user.Id.ToString(), user.GetFullName(), user.Email, user.Names, user.Surname, token, user.TenantId.ToString());
    }

    public async Task<AuthResponse> ExchangeTenantAsync(Guid userId, string newTenantId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            throw new Exception("User Not Found");
        }

        if (user.IsBanned)
        {
            throw new Exception("User is banned");
        }

        // Check 7-day rule
        if (user.NextAvailableTenantSwitch.HasValue && user.NextAvailableTenantSwitch.Value > DateTimeOffset.UtcNow)
        {
            var diff = user.NextAvailableTenantSwitch.Value - DateTimeOffset.UtcNow;
            var remainingDays = Math.Ceiling(diff.TotalDays);
            throw new Exception($"You can only change teams every 7 days. Please wait {remainingDays} more days.");
        }

        if (!Guid.TryParse(newTenantId, out var parsedTenantId))
        {
            throw new Exception("Invalid tenant ID format");
        }

        // Apply new tenant and enforce lock
        user.TenantId = parsedTenantId;
        user.NextAvailableTenantSwitch = DateTimeOffset.UtcNow.AddDays(7);
        user.LastActiveAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var token = GenerateJwtToken(user);
        return new AuthResponse(user.Id.ToString(), user.GetFullName(), user.Email, user.Names, user.Surname, token, user.TenantId.ToString());
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var appName = _configuration.GetValue<string>("Name") ?? "unknown";
        var secretKey = jwtSettings["Secret"] ?? "SuperSecretKey12345678901234567890"; // Fallback for dev
        var key = Encoding.ASCII.GetBytes(secretKey);
        var fullNames = user.Username;
        if (!string.IsNullOrEmpty(user.Names) && !string.IsNullOrEmpty(user.Surname))
        {
            fullNames = $"{user.Names} {user.Surname}";
        }
        else if (!string.IsNullOrEmpty(user.Names))
        {
            fullNames = user.Names;
        }
        else if (!string.IsNullOrEmpty(user.Surname))
        {
            fullNames = user.Surname;
        }
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, fullNames),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Actor, appName),
                new Claim("tenantId", user.TenantId.ToString()),
            }),
            Expires = DateTime.UtcNow.AddMonths(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string HashPassword(string password, Guid userId)
    {
        var list = userId.ToString().ToLower().ToList();
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"{password}_{string.Join("", list)}"));
        return Convert.ToBase64String(bytes);
    }

    public bool VerifyPassword(string password, string storedHash, Guid userId)
    {
        var hash = HashPassword(password, userId);
        return hash == storedHash;
    }
}