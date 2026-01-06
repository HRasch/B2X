namespace B2Connect.Admin.Application.Handlers;

/// <summary>
/// Query handler interface for Wolverine CQRS pattern
/// Handlers implement this interface to handle query messages
/// </summary>
/// <typeparam name="TQuery">The query type to handle</typeparam>
/// <typeparam name="TResult">The result type returned after handling</typeparam>
public interface IQueryHandler<TQuery, TResult>
{
    /// <summary>
    /// Handle the query and return a result
    /// </summary>
    /// <param name="query">The query to handle</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of handling the query</returns>
    Task<TResult> Handle(TQuery query, CancellationToken cancellationToken = default);
}
