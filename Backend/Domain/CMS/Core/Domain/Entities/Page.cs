using System;

namespace B2Connect.CMS.Core.Domain.Entities
{
    /// <summary>
    /// Represents a CMS page entity
    /// </summary>
    public class Page : AggregateRoot
    {
        public string TenantId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Page()
        {
        }

        public Page(string tenantId, string title, string slug, string content)
        {
            TenantId = tenantId;
            Title = title;
            Slug = slug;
            Content = content;
            IsPublished = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}