using MediatR;
using FluentValidation;

namespace B2Connect.Shared.User.Application.Commands;

/// <summary>
/// Command: Erstelle einen User mit ERP-Integration
/// </summary>
public record CreateUserWithErpCommand(
    Guid TenantId,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string? ErpCustomerId,
    string? ErpApiKey,
    string? ErpBaseUrl
) : IRequest<UserDto>;

/// <summary>
/// Handler für User-Erstellung mit Custom Properties (ERP)
/// </summary>
public class CreateUserWithErpHandler : IRequestHandler<CreateUserWithErpCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IEntityExtensionService _extensionService;
    private readonly EnventaTradeEerIntegration _erpIntegration;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserWithErpHandler> _logger;

    public CreateUserWithErpHandler(
        IUserRepository userRepository,
        IEntityExtensionService extensionService,
        EnventaTradeEerIntegration erpIntegration,
        IMapper mapper,
        ILogger<CreateUserWithErpHandler> logger)
    {
        _userRepository = userRepository;
        _extensionService = extensionService;
        _erpIntegration = erpIntegration;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto> Handle(CreateUserWithErpCommand request, CancellationToken ct)
    {
        // Validiere ob User bereits existiert
        var existingUser = await _userRepository.GetByEmailAsync(request.TenantId, request.Email, ct);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"User with email {request.Email} already exists");
        }

        // Erstelle User Entity
        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Wenn ERP-Integration konfiguriert, synch Custom Properties
        if (!string.IsNullOrEmpty(request.ErpCustomerId) &&
            !string.IsNullOrEmpty(request.ErpApiKey) &&
            !string.IsNullOrEmpty(request.ErpBaseUrl))
        {
            try
            {
                await _erpIntegration.SyncUserWithErpAsync(
                    user,
                    request.ErpCustomerId,
                    request.ErpApiKey,
                    request.ErpBaseUrl);

                _logger.LogInformation(
                    "User {UserId} synchronized with ERP customer {ErpCustomerId}",
                    user.Id, request.ErpCustomerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to synchronize with ERP for customer {ErpCustomerId}",
                    request.ErpCustomerId);
                // Optional: Rethrow oder continue ohne ERP-Daten
                // throw;
            }
        }

        // Speichere User
        await _userRepository.AddAsync(user, ct);
        await _userRepository.SaveChangesAsync(ct);

        // Mapping zu DTO
        var userDto = _mapper.Map<UserDto>(user);

        // Füge Custom Properties zum DTO hinzu
        var customProps = _extensionService.GetAllCustomProperties(user);
        userDto.CustomProperties = customProps;

        return userDto;
    }
}

/// <summary>
/// Validator für Create User Command
/// </summary>
public class CreateUserWithErpCommandValidator : AbstractValidator<CreateUserWithErpCommand>
{
    public CreateUserWithErpCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        // If ERP integration provided, all ERP fields must be provided
        RuleFor(x => x.ErpCustomerId)
            .NotEmpty().WithMessage("ERP Customer ID is required")
            .When(x => !string.IsNullOrEmpty(x.ErpApiKey) || !string.IsNullOrEmpty(x.ErpBaseUrl));

        RuleFor(x => x.ErpApiKey)
            .NotEmpty().WithMessage("ERP API Key is required")
            .When(x => !string.IsNullOrEmpty(x.ErpCustomerId));

        RuleFor(x => x.ErpBaseUrl)
            .NotEmpty().WithMessage("ERP Base URL is required")
            .Must(x => string.IsNullOrEmpty(x) || Uri.IsWellFormedUriString(x, UriKind.Absolute))
            .WithMessage("ERP Base URL must be a valid URL");
    }
}

/// <summary>
/// DTO mit Custom Properties Support
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Tenant-spezifische Custom Properties
    /// Beispiel: { "erp_customer_id": "12345", "erp_customer_number": "CUST-123456" }
    /// </summary>
    public Dictionary<string, object?> CustomProperties { get; set; } = new();
}
