namespace B2Connect.Email.Models;

/// <summary>
/// DTO für Email-Template Erstellung
/// </summary>
public class CreateEmailTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
    public Dictionary<string, string> Variables { get; set; } = new(StringComparer.Ordinal);
    public string? Description { get; set; }
}
