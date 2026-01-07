using B2Connect.LayoutService.Data;
using B2Connect.LayoutService.Models;

namespace B2Connect.LayoutService.Data
{
    /// <summary>
    /// Request to add a new section to a page
    /// </summary>
    public class AddSectionRequest
    {
        /// <summary>
        /// The type of section (e.g., "hero", "content", "footer")
        /// </summary>
        public required string Type { get; set; }

        /// <summary>
        /// The layout configuration for the section
        /// </summary>
        public required SectionLayout Layout { get; set; }

        /// <summary>
        /// Additional settings for the section
        /// </summary>
        public Dictionary<string, object>? Settings { get; set; }
    }
}
