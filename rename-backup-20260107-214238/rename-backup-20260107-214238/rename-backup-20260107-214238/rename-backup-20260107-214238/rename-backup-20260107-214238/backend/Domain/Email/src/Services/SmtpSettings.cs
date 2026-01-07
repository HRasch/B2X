namespace B2Connect.Email.Services;

/// <summary>
/// SMTP-Konfiguration
/// </summary>
public class SmtpSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
    public int Timeout { get; set; } = 30000; // 30 seconds
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string Domain { get; set; } = "b2connect.local";
}
