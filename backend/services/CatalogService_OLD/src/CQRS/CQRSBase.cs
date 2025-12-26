using Wolverine;
using B2Connect.CatalogService.CQRS;

namespace B2Connect.CatalogService.CQRS;

/// <summary>
/// Message Bus for sending commands and queries
/// Wolverine's central messaging hub
/// </summary>
public interface IMessageBus
{
    /// <summary>Invoke a command and wait for response</summary>
    Task<TResponse> InvokeAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct = default);

    /// <summary>Invoke a query and wait for response</summary>
    Task<TResponse> InvokeAsync<TQuery, TResponse>(TQuery query, CancellationToken ct = default)
        where TQuery : IQuery<TResponse>;

    /// <summary>Send a command without waiting for response</summary>
    Task SendAsync(ICommand command, CancellationToken ct = default);

    /// <summary>Publish an event to all subscribers</summary>
    Task PublishAsync(object @event, CancellationToken ct = default);
}

/// <summary>
/// Base marker interface for all commands
/// Wolverine automatically discovers ICommandHandler<T> implementations
/// </summary>
public interface ICommand { }

/// <summary>
/// Handler for commands
/// </summary>
/// <typeparam name="TCommand">Command type</typeparam>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    /// <summary>Handle the command</summary>
    Task Handle(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handler for commands with response
/// </summary>
/// <typeparam name="TCommand">Command type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    /// <summary>Handle the command and return response</summary>
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handler for events
/// </summary>
/// <typeparam name="TEvent">Event type</typeparam>
public interface IEventHandler<in TEvent>
{
    /// <summary>Handle the event</summary>
    Task Handle(TEvent @event, CancellationToken cancellationToken = default);
}

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
/// Handler for queries
/// Queries are always synchronous request/reply via Wolverine
/// </summary>
/// <typeparam name="TQuery">Query type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// Handle the query and return result
    /// </summary>
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken = default);
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
