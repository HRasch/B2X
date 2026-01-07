using System.Net.Http.Json;
using System.Text.Json;
using B2Connect.Admin.Application.Commands.Users;
using B2Connect.Admin.Application.Handlers;
using B2Connect.Shared.Middleware;

namespace B2Connect.Admin.Application.Handlers.Users;

/// <summary>
/// Wolverine Message Handlers für User Commands/Queries
/// Diese Handlers kapseln die HTTP-Kommunikation mit dem Identity Service
///
/// Flow:
/// Controller → IMessageBus.InvokeAsync → Handler → Identity Service (HTTP)
///
/// Benefits:
/// - Business Logic im Handler (nicht im Controller)
/// - Testbar (Handler können gemockt werden)
/// - Konsistent mit anderen Bounded Contexts (Products, Categories, Brands)
/// - TenantId automatisch via ITenantContextAccessor (aus Middleware)
/// </summary>
public class GetUsersHandler : IQueryHandler<GetUsersQuery, UsersListResult?>
{
    private readonly HttpClient _httpClient;
    private readonly string _identityServiceUrl;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<GetUsersHandler> _logger;

    public GetUsersHandler(
        HttpClient httpClient,
        IConfiguration configuration,
        ITenantContextAccessor tenantContext,
        ILogger<GetUsersHandler> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _identityServiceUrl = configuration["Services:Identity:Url"] ?? "http://localhost:7002";
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UsersListResult?> Handle(GetUsersQuery query, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.GetTenantId();
        _logger.LogInformation("Fetching users for tenant {TenantId}", tenantId);

        var url = $"{_identityServiceUrl}/api/identity/users";

        // Add tenant header
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-Tenant-ID", tenantId.ToString());

        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Identity Service returned {StatusCode}", response.StatusCode);
            return null;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var users = JsonSerializer.Deserialize<IEnumerable<UserResult>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return new UsersListResult(users ?? Enumerable.Empty<UserResult>());
    }
}

public class GetUserHandler : IQueryHandler<GetUserQuery, UserResult?>
{
    private readonly HttpClient _httpClient;
    private readonly string _identityServiceUrl;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<GetUserHandler> _logger;

    public GetUserHandler(
        HttpClient httpClient,
        IConfiguration configuration,
        ITenantContextAccessor tenantContext,
        ILogger<GetUserHandler> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _identityServiceUrl = configuration["Services:Identity:Url"] ?? "http://localhost:7002";
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UserResult?> Handle(GetUserQuery query, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.GetTenantId();
        _logger.LogInformation("Fetching user {UserId} for tenant {TenantId}", query.UserId, tenantId);

        var url = $"{_identityServiceUrl}/api/identity/users/{query.UserId}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-Tenant-ID", tenantId.ToString());

        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("User {UserId} not found", query.UserId);
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Identity Service returned {StatusCode}", response.StatusCode);
            return null;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<UserResult>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}

public class CreateUserHandler : ICommandHandler<CreateUserCommand, UserResult?>
{
    private readonly HttpClient _httpClient;
    private readonly string _identityServiceUrl;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<CreateUserHandler> _logger;

    public CreateUserHandler(
        HttpClient httpClient,
        IConfiguration configuration,
        ITenantContextAccessor tenantContext,
        ILogger<CreateUserHandler> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _identityServiceUrl = configuration["Services:Identity:Url"] ?? "http://localhost:7002";
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UserResult?> Handle(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.GetTenantId();
        _logger.LogInformation(
            "User {CreatedByUserId} creating new user for tenant {TenantId}",
            command.CreatedByUserId, tenantId);

        var url = $"{_identityServiceUrl}/api/identity/users";

        var requestBody = new
        {
            email = command.Email,
            firstName = command.FirstName,
            lastName = command.LastName,
            password = command.Password,
            roles = command.Roles
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(requestBody)
        };
        request.Headers.Add("X-Tenant-ID", tenantId.ToString());

        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Identity Service returned {StatusCode}", response.StatusCode);
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new InvalidOperationException($"Failed to create user: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<UserResult>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}

public class UpdateUserHandler : ICommandHandler<UpdateUserCommand, UserResult?>
{
    private readonly HttpClient _httpClient;
    private readonly string _identityServiceUrl;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<UpdateUserHandler> _logger;

    public UpdateUserHandler(
        HttpClient httpClient,
        IConfiguration configuration,
        ITenantContextAccessor tenantContext,
        ILogger<UpdateUserHandler> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _identityServiceUrl = configuration["Services:Identity:Url"] ?? "http://localhost:7002";
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UserResult?> Handle(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.GetTenantId();
        _logger.LogInformation(
            "User {UpdatedByUserId} updating user {UserId} for tenant {TenantId}",
            command.UpdatedByUserId, command.UserId, tenantId);

        var url = $"{_identityServiceUrl}/api/identity/users/{command.UserId}";

        var requestBody = new
        {
            email = command.Email,
            firstName = command.FirstName,
            lastName = command.LastName,
            isActive = command.IsActive,
            roles = command.Roles
        };

        using var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = JsonContent.Create(requestBody)
        };
        request.Headers.Add("X-Tenant-ID", tenantId.ToString());

        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Identity Service returned {StatusCode}", response.StatusCode);
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new InvalidOperationException($"Failed to update user: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<UserResult>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}

public class DeleteUserHandler : ICommandHandler<DeleteUserCommand, bool>
{
    private readonly HttpClient _httpClient;
    private readonly string _identityServiceUrl;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<DeleteUserHandler> _logger;

    public DeleteUserHandler(
        HttpClient httpClient,
        IConfiguration configuration,
        ITenantContextAccessor tenantContext,
        ILogger<DeleteUserHandler> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _identityServiceUrl = configuration["Services:Identity:Url"] ?? "http://localhost:7002";
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(DeleteUserCommand command, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantContext.GetTenantId();
        _logger.LogInformation(
            "User {DeletedByUserId} deleting user {UserId} for tenant {TenantId}",
            command.DeletedByUserId, command.UserId, tenantId);

        var url = $"{_identityServiceUrl}/api/identity/users/{command.UserId}";

        using var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.Add("X-Tenant-ID", tenantId.ToString());

        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return false;
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Identity Service returned {StatusCode}", response.StatusCode);
            throw new InvalidOperationException($"Failed to delete user: {response.StatusCode}");
        }

        _logger.LogInformation("User {UserId} deleted successfully", command.UserId);
        return true;
    }
}
