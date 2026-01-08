using System;

namespace B2X.SmartDataIntegration.Models
{
    /// <summary>
    /// Represents a data field in a source or target system
    /// </summary>
    public class DataField
    {
        /// <summary>
        /// Name of the field
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Display name for the field
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Data type of the field
        /// </summary>
        public DataFieldType DataType { get; set; }

        /// <summary>
        /// Maximum length for string fields
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Whether the field is required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Whether the field is a primary key
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Default value for the field
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Description of the field
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Sample values for AI analysis
        /// </summary>
        public string[]? SampleValues { get; set; }

        /// <summary>
        /// Business context information
        /// </summary>
        public string? BusinessContext { get; set; }

        /// <summary>
        /// Creates a copy of this field
        /// </summary>
        public DataField Clone()
        {
            return new DataField
            {
                Name = Name,
                DisplayName = DisplayName,
                DataType = DataType,
                MaxLength = MaxLength,
                IsRequired = IsRequired,
                IsPrimaryKey = IsPrimaryKey,
                DefaultValue = DefaultValue,
                Description = Description,
                SampleValues = SampleValues?.ToArray(),
                BusinessContext = BusinessContext
            };
        }
    }

    /// <summary>
    /// Data types supported for field mapping
    /// </summary>
    public enum DataFieldType
    {
        /// <summary>
        /// String/text data
        /// </summary>
        String = 0,

        /// <summary>
        /// Integer number
        /// </summary>
        Integer = 1,

        /// <summary>
        /// Decimal number
        /// </summary>
        Decimal = 2,

        /// <summary>
        /// Boolean value
        /// </summary>
        Boolean = 3,

        /// <summary>
        /// Date value
        /// </summary>
        Date = 4,

        /// <summary>
        /// Date and time value
        /// </summary>
        DateTime = 5,

        /// <summary>
        /// JSON object
        /// </summary>
        Json = 6,

        /// <summary>
        /// Binary data
        /// </summary>
        Binary = 7,

        /// <summary>
        /// Unique identifier (GUID)
        /// </summary>
        Guid = 8
    }
}
