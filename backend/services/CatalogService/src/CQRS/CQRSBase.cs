using Wolverine;

namespace B2Connect.CatalogService.CQRS;

/// <summary>
/// Base marker interface for all commands
/// Wolverine automatically discovers ICommandHandler<T> implementations
/// </summary>
public interface ICommand { }

/// <summary>
/// Command with response type
/// Wolverine uses Request/Reply pattern for synchronous commands
/// </summary>
public interface ICommand<out TResponse> : ICommand { }

/// <summary>
/// Base marker interface for all queries
/// Queries are always synchronous request/reply via Wolverine
/// </summary>
public interface IQuery<out TResponse> { }

/// <summary>
/// Standard command result
/// Returned by most command handlers to indicate success/failure
/// </summary>
public class CommandResult
{
    public Guid Id { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    public static CommandResult Ok(Guid id) =>
        new() { Id = id, Success = true };

    public static CommandResult Fail(string error) =>
        new() { Success = false, ErrorMessage = error };

    public static CommandResult Fail(IEnumerable<string> errors) =>
        new() { Success = false, Errors = errors };
}

/// <summary>
/// Paged result for query responses
/// Includes pagination metadata and items
/// </summary>
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
