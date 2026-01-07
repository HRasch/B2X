namespace B2Connect.Admin.Application.Handlers;

/// <summary>
/// Command handler interface for Wolverine CQRS pattern
/// Handlers implement this interface to handle command messages
/// </summary>
/// <typeparam name="TCommand">The command type to handle</typeparam>
/// <typeparam name="TResult">The result type returned after handling</typeparam>
public interface ICommandHandler<TCommand, TResult>
{
    /// <summary>
    /// Handle the command and return a result
    /// </summary>
    /// <param name="command">The command to handle</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of handling the command</returns>
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default);
}
