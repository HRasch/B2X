namespace B2Connect.Email.Models;

/// <summary>
/// DTO für Email-Template Aktualisierung
/// </summary>
public class UpdateEmailTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
    public Dictionary<string, string> Variables { get; set; } = new(StringComparer.Ordinal);
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
