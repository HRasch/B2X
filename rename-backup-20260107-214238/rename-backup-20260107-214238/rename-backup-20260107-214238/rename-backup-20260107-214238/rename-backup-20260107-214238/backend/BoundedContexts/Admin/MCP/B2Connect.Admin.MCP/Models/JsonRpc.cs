namespace B2Connect.Admin.MCP.Models;

public class JsonRpcRequest
{
    public string Jsonrpc { get; set; } = "2.0";
    public string Id { get; set; } = null!;
    public string Method { get; set; } = null!;
    public object? Params { get; set; }
}

public class JsonRpcResponse
{
    public string Jsonrpc { get; set; } = "2.0";
    public string Id { get; set; } = null!;
    public object? Result { get; set; }
    public JsonRpcError? Error { get; set; }
}

public class JsonRpcError
{
    public int Code { get; set; }
    public string Message { get; set; } = null!;
    public object? Data { get; set; }
}