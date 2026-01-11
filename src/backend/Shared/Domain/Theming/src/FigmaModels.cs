using System.Text.Json.Serialization;

namespace B2X.ThemeService.Models;

/// <summary>
/// Figma file data response from API
/// </summary>
public class FigmaFileData
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("lastModified")]
    public DateTime? LastModified { get; set; }

    [JsonPropertyName("thumbnailUrl")]
    public string? ThumbnailUrl { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("editorType")]
    public string? EditorType { get; set; }

    [JsonPropertyName("linkAccess")]
    public string? LinkAccess { get; set; }

    [JsonPropertyName("document")]
    public FigmaNode? Document { get; set; }

    [JsonPropertyName("components")]
    public Dictionary<string, FigmaComponent>? Components { get; set; }

    [JsonPropertyName("componentSets")]
    public Dictionary<string, FigmaComponentSet>? ComponentSets { get; set; }

    [JsonPropertyName("schemaVersion")]
    public int? SchemaVersion { get; set; }

    [JsonPropertyName("styles")]
    public Dictionary<string, FigmaStyle>? Styles { get; set; }

    // For backwards compatibility - some older files might have nodes at root
    [JsonPropertyName("nodes")]
    public Dictionary<string, FigmaNode>? Nodes { get; set; }

    // Figma Variables API (if available)
    [JsonPropertyName("variables")]
    public List<FigmaVariable>? Variables { get; set; }
}

/// <summary>
/// Figma node representation
/// </summary>
public class FigmaNode
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("children")]
    public List<FigmaNode>? Children { get; set; }

    [JsonPropertyName("backgroundColor")]
    public FigmaColor? BackgroundColor { get; set; }

    [JsonPropertyName("fills")]
    public List<FigmaPaint>? Fills { get; set; }

    [JsonPropertyName("strokes")]
    public List<FigmaPaint>? Strokes { get; set; }

    [JsonPropertyName("strokeWeight")]
    public double? StrokeWeight { get; set; }

    [JsonPropertyName("strokeAlign")]
    public string? StrokeAlign { get; set; }

    [JsonPropertyName("cornerRadius")]
    public double? CornerRadius { get; set; }

    [JsonPropertyName("rectangleCornerRadii")]
    public List<double>? RectangleCornerRadii { get; set; }

    [JsonPropertyName("opacity")]
    public double? Opacity { get; set; }

    [JsonPropertyName("blendMode")]
    public string? BlendMode { get; set; }

    [JsonPropertyName("isMask")]
    public bool? IsMask { get; set; }

    [JsonPropertyName("effects")]
    public List<FigmaEffect>? Effects { get; set; }

    [JsonPropertyName("style")]
    public FigmaTypeStyle? Style { get; set; }

    [JsonPropertyName("layoutAlign")]
    public string? LayoutAlign { get; set; }

    [JsonPropertyName("layoutGrow")]
    public double? LayoutGrow { get; set; }

    [JsonPropertyName("layoutMode")]
    public string? LayoutMode { get; set; }

    [JsonPropertyName("primaryAxisSizingMode")]
    public string? PrimaryAxisSizingMode { get; set; }

    [JsonPropertyName("counterAxisSizingMode")]
    public string? CounterAxisSizingMode { get; set; }

    [JsonPropertyName("primaryAxisAlignItems")]
    public string? PrimaryAxisAlignItems { get; set; }

    [JsonPropertyName("counterAxisAlignItems")]
    public string? CounterAxisAlignItems { get; set; }

    [JsonPropertyName("paddingLeft")]
    public double? PaddingLeft { get; set; }

    [JsonPropertyName("paddingRight")]
    public double? PaddingRight { get; set; }

    [JsonPropertyName("paddingTop")]
    public double? PaddingTop { get; set; }

    [JsonPropertyName("paddingBottom")]
    public double? PaddingBottom { get; set; }

    [JsonPropertyName("horizontalPadding")]
    public double? HorizontalPadding { get; set; }

    [JsonPropertyName("verticalPadding")]
    public double? VerticalPadding { get; set; }

    [JsonPropertyName("itemSpacing")]
    public double? ItemSpacing { get; set; }

    [JsonPropertyName("layoutGrids")]
    public List<FigmaLayoutGrid>? LayoutGrids { get; set; }

    [JsonPropertyName("overflowDirection")]
    public string? OverflowDirection { get; set; }

    [JsonPropertyName("size")]
    public FigmaVector? Size { get; set; }

    [JsonPropertyName("minSize")]
    public FigmaVector? MinSize { get; set; }

    [JsonPropertyName("maxSize")]
    public FigmaVector? MaxSize { get; set; }

    [JsonPropertyName("relativeTransform")]
    public List<List<double>>? RelativeTransform { get; set; }

    [JsonPropertyName("absoluteTransform")]
    public List<List<double>>? AbsoluteTransform { get; set; }

    [JsonPropertyName("absoluteRenderBounds")]
    public FigmaRectangle? AbsoluteRenderBounds { get; set; }

    [JsonPropertyName("absoluteBoundingBox")]
    public FigmaRectangle? AbsoluteBoundingBox { get; set; }

    [JsonPropertyName("constraints")]
    public FigmaLayoutConstraint? Constraints { get; set; }

    [JsonPropertyName("clipsContent")]
    public bool? ClipsContent { get; set; }

    [JsonPropertyName("background")]
    public List<FigmaPaint>? Background { get; set; }

    [JsonPropertyName("backgrounds")]
    public List<FigmaPaint>? Backgrounds { get; set; }

    [JsonPropertyName("characters")]
    public string? Characters { get; set; }

    [JsonPropertyName("styleOverrideTable")]
    public Dictionary<string, FigmaTypeStyle>? StyleOverrideTable { get; set; }

    [JsonPropertyName("componentId")]
    public string? ComponentId { get; set; }

    [JsonPropertyName("componentProperties")]
    public Dictionary<string, FigmaComponentProperty>? ComponentProperties { get; set; }
}

/// <summary>
/// Figma color representation
/// </summary>
public class FigmaColor
{
    [JsonPropertyName("r")]
    public double R { get; set; }

    [JsonPropertyName("g")]
    public double G { get; set; }

    [JsonPropertyName("b")]
    public double B { get; set; }

    [JsonPropertyName("a")]
    public double A { get; set; } = 1.0;
}

/// <summary>
/// Figma paint (fill/stroke) representation
/// </summary>
public class FigmaPaint
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("visible")]
    public bool? Visible { get; set; } = true;

    [JsonPropertyName("opacity")]
    public double? Opacity { get; set; } = 1.0;

    [JsonPropertyName("color")]
    public FigmaColor? Color { get; set; }

    [JsonPropertyName("blendMode")]
    public string? BlendMode { get; set; }

    [JsonPropertyName("scaleMode")]
    public string? ScaleMode { get; set; }

    [JsonPropertyName("imageRef")]
    public string? ImageRef { get; set; }

    [JsonPropertyName("filters")]
    public FigmaImageFilters? Filters { get; set; }

    [JsonPropertyName("gradientStops")]
    public List<FigmaGradientStop>? GradientStops { get; set; }

    [JsonPropertyName("gradientHandlePositions")]
    public List<FigmaVector>? GradientHandlePositions { get; set; }
}

/// <summary>
/// Figma type style for text
/// </summary>
public class FigmaTypeStyle
{
    [JsonPropertyName("fontFamily")]
    public string? FontFamily { get; set; }

    [JsonPropertyName("fontPostScriptName")]
    public string? FontPostScriptName { get; set; }

    [JsonPropertyName("fontWeight")]
    public double? FontWeight { get; set; }

    [JsonPropertyName("fontSize")]
    public double? FontSize { get; set; }

    [JsonPropertyName("textAlignHorizontal")]
    public string? TextAlignHorizontal { get; set; }

    [JsonPropertyName("textAlignVertical")]
    public string? TextAlignVertical { get; set; }

    [JsonPropertyName("letterSpacing")]
    public double? LetterSpacing { get; set; }

    [JsonPropertyName("lineHeightPx")]
    public double? LineHeightPx { get; set; }

    [JsonPropertyName("lineHeightPercent")]
    public double? LineHeightPercent { get; set; }

    [JsonPropertyName("lineHeightPercentFontSize")]
    public double? LineHeightPercentFontSize { get; set; }

    [JsonPropertyName("lineHeightUnit")]
    public string? LineHeightUnit { get; set; }
}

/// <summary>
/// Figma variable (from Variables API)
/// </summary>
public class FigmaVariable
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    [JsonPropertyName("scopes")]
    public List<string>? Scopes { get; set; }

    [JsonPropertyName("codeSyntax")]
    public FigmaCodeSyntax? CodeSyntax { get; set; }
}

/// <summary>
/// Figma code syntax for variables
/// </summary>
public class FigmaCodeSyntax
{
    [JsonPropertyName("WEB")]
    public string? Web { get; set; }

    [JsonPropertyName("ANDROID")]
    public string? Android { get; set; }

    [JsonPropertyName("iOS")]
    public string? IOS { get; set; }
}

/// <summary>
/// Additional Figma model classes for completeness
/// </summary>
public class FigmaComponent
{
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("componentSetId")]
    public string? ComponentSetId { get; set; }

    [JsonPropertyName("documentationLinks")]
    public List<FigmaDocumentationLink>? DocumentationLinks { get; set; }
}

public class FigmaComponentSet
{
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class FigmaStyle
{
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("styleType")]
    public string? StyleType { get; set; }
}

public class FigmaEffect
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("visible")]
    public bool? Visible { get; set; }

    [JsonPropertyName("radius")]
    public double? Radius { get; set; }

    [JsonPropertyName("color")]
    public FigmaColor? Color { get; set; }

    [JsonPropertyName("blendMode")]
    public string? BlendMode { get; set; }

    [JsonPropertyName("offset")]
    public FigmaVector? Offset { get; set; }
}

public class FigmaLayoutGrid
{
    [JsonPropertyName("pattern")]
    public string? Pattern { get; set; }

    [JsonPropertyName("sectionSize")]
    public double? SectionSize { get; set; }

    [JsonPropertyName("visible")]
    public bool? Visible { get; set; }

    [JsonPropertyName("color")]
    public FigmaColor? Color { get; set; }

    [JsonPropertyName("alignment")]
    public string? Alignment { get; set; }

    [JsonPropertyName("gutterSize")]
    public double? GutterSize { get; set; }

    [JsonPropertyName("offset")]
    public double? Offset { get; set; }

    [JsonPropertyName("count")]
    public double? Count { get; set; }
}

public class FigmaVector
{
    [JsonPropertyName("x")]
    public double? X { get; set; }

    [JsonPropertyName("y")]
    public double? Y { get; set; }
}

public class FigmaRectangle
{
    [JsonPropertyName("x")]
    public double? X { get; set; }

    [JsonPropertyName("y")]
    public double? Y { get; set; }

    [JsonPropertyName("width")]
    public double? Width { get; set; }

    [JsonPropertyName("height")]
    public double? Height { get; set; }
}

public class FigmaLayoutConstraint
{
    [JsonPropertyName("vertical")]
    public string? Vertical { get; set; }

    [JsonPropertyName("horizontal")]
    public string? Horizontal { get; set; }
}

public class FigmaImageFilters
{
    [JsonPropertyName("exposure")]
    public double? Exposure { get; set; }

    [JsonPropertyName("contrast")]
    public double? Contrast { get; set; }

    [JsonPropertyName("saturation")]
    public double? Saturation { get; set; }

    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName("tint")]
    public double? Tint { get; set; }

    [JsonPropertyName("highlights")]
    public double? Highlights { get; set; }

    [JsonPropertyName("shadows")]
    public double? Shadows { get; set; }
}

public class FigmaGradientStop
{
    [JsonPropertyName("color")]
    public FigmaColor? Color { get; set; }

    [JsonPropertyName("position")]
    public double? Position { get; set; }
}

public class FigmaComponentProperty
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    [JsonPropertyName("preferredValues")]
    public List<object>? PreferredValues { get; set; }
}

public class FigmaDocumentationLink
{
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }
}
