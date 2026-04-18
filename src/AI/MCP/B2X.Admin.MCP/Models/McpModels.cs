namespace B2X.Admin.MCP.Models;

public class McpInitializeRequest
{
    public Implementation Server { get; set; } = null!;
    public Capabilities Capabilities { get; set; } = null!;
}

public class Implementation
{
    public string Name { get; set; } = null!;
    public string Version { get; set; } = null!;
}

public class Capabilities
{
    public ToolsCapability? Tools { get; set; }
    public ResourcesCapability? Resources { get; set; }
}

public class ToolsCapability
{
    public bool ListChanged { get; set; } = false;
}

public class ResourcesCapability
{
    public bool Subscribe { get; set; } = false;
    public bool ListChanged { get; set; } = false;
}

public class McpInitializeResult
{
    public string ProtocolVersion { get; set; } = "2024-11-05";
    public Implementation Server { get; set; } = null!;
    public Capabilities Capabilities { get; set; } = null!;
}

public class Tool
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public object? InputSchema { get; set; }
}

public class ListToolsResult
{
    public Tool[] Tools { get; set; } = Array.Empty<Tool>();
}

public class CallToolRequest
{
    public string Name { get; set; } = null!;
    public object? Arguments { get; set; }
}

public class CallToolResult
{
    public object? Content { get; set; }
    public bool IsError { get; set; } = false;
}
