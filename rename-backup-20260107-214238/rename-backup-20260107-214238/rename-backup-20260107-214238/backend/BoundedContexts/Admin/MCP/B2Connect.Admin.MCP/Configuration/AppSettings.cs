namespace B2X.Admin.MCP.Configuration;

/// <summary>
/// Application configuration settings
/// </summary>
public class AppSettings
{
    public JwtSettings Jwt { get; set; } = new();
    public DatabaseSettings Database { get; set; } = new();
    public AzureSettings Azure { get; set; } = new();
    public AiSettings Ai { get; set; } = new();
    public CacheSettings Cache { get; set; } = new();
}

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public int MaxRetryCount { get; set; } = 3;
    public int CommandTimeout { get; set; } = 30;
}

public class AzureSettings
{
    public KeyVaultSettings KeyVault { get; set; } = new();
}

public class KeyVaultSettings
{
    public string Url { get; set; } = string.Empty;
}

public class AiSettings
{
    public ConsumptionSettings Consumption { get; set; } = new();
    public ProviderSettings Providers { get; set; } = new();
}

public class ConsumptionSettings
{
    public int DefaultRateLimitPerMinute { get; set; } = 100;
    public decimal DefaultMonthlyBudget { get; set; } = 1000m;
    public int CacheExpirationHours { get; set; } = 24;
}

public class ProviderSettings
{
    public OpenAiSettings OpenAI { get; set; } = new();
    public AnthropicSettings Anthropic { get; set; } = new();
    public AzureOpenAiSettings AzureOpenAI { get; set; } = new();
}

public class OpenAiSettings
{
    public string BaseUrl { get; set; } = "https://api.openai.com/v1/";
    public string DefaultModel { get; set; } = "gpt-4";
    public bool Enabled { get; set; } = true;
}

public class AnthropicSettings
{
    public string BaseUrl { get; set; } = "https://api.anthropic.com/";
    public string DefaultModel { get; set; } = "claude-3-sonnet-20240229";
    public bool Enabled { get; set; } = true;
}

public class AzureOpenAiSettings
{
    public string DefaultEndpoint { get; set; } = string.Empty;
    public string DefaultModel { get; set; } = "gpt-4";
    public bool Enabled { get; set; } = true;
}

public class CacheSettings
{
    public string RedisConnectionString { get; set; } = string.Empty;
    public int DefaultExpirationMinutes { get; set; } = 60;
}