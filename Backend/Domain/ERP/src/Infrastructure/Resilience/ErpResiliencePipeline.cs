// <copyright file="ErpResiliencePipeline.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace B2Connect.ERP.Infrastructure.Resilience;

/// <summary>
/// Resilience pipeline for ERP operations using Polly.
/// Provides Circuit Breaker, Retry, and Timeout policies.
/// </summary>
public sealed class ErpResiliencePipeline : IDisposable
{
    private readonly ResiliencePipeline _pipeline;
    private readonly ILogger<ErpResiliencePipeline> _logger;
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErpResiliencePipeline"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="options">Optional resilience options.</param>
    public ErpResiliencePipeline(
        ILogger<ErpResiliencePipeline> logger,
        ErpResilienceOptions? options = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        options ??= ErpResilienceOptions.Default;

        _pipeline = new ResiliencePipelineBuilder()
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = options.OperationTimeout,
                OnTimeout = args =>
                {
                    _logger.LogWarning(
                        "ERP operation timed out after {Timeout}s",
                        args.Timeout.TotalSeconds);
                    return default;
                }
            })
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = options.MaxRetryAttempts,
                Delay = options.RetryDelay,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = new PredicateBuilder()
                    .Handle<ErpTransientException>()
                    .Handle<TimeoutException>()
                    .Handle<OperationCanceledException>(ex => !ex.CancellationToken.IsCancellationRequested),
                OnRetry = args =>
                {
                    _logger.LogWarning(
                        args.Outcome.Exception,
                        "ERP operation retry {Attempt}/{MaxAttempts} after {Delay}ms",
                        args.AttemptNumber,
                        options.MaxRetryAttempts,
                        args.RetryDelay.TotalMilliseconds);
                    return default;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = options.CircuitBreakerFailureRatio,
                SamplingDuration = options.CircuitBreakerSamplingDuration,
                MinimumThroughput = options.CircuitBreakerMinimumThroughput,
                BreakDuration = options.CircuitBreakerBreakDuration,
                ShouldHandle = new PredicateBuilder()
                    .Handle<ErpConnectionException>()
                    .Handle<ErpTransientException>()
                    .Handle<TimeoutException>(),
                OnOpened = args =>
                {
                    _logger.LogError(
                        args.Outcome.Exception,
                        "Circuit breaker OPENED for ERP operations. Break duration: {Duration}s",
                        args.BreakDuration.TotalSeconds);
                    return default;
                },
                OnClosed = args =>
                {
                    _logger.LogInformation("Circuit breaker CLOSED for ERP operations");
                    return default;
                },
                OnHalfOpened = args =>
                {
                    _logger.LogInformation("Circuit breaker HALF-OPEN for ERP operations (testing)");
                    return default;
                }
            })
            .Build();
    }

    /// <summary>
    /// Gets the current circuit breaker state.
    /// </summary>
    public CircuitState CircuitState { get; private set; } = CircuitState.Closed;

    /// <summary>
    /// Executes an operation through the resilience pipeline.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return await _pipeline.ExecuteAsync(
            async ct => await operation(ct).ConfigureAwait(false),
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes an operation through the resilience pipeline.
    /// </summary>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task ExecuteAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        await _pipeline.ExecuteAsync(
            async ct =>
            {
                await operation(ct).ConfigureAwait(false);
            },
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
    }
}

/// <summary>
/// Options for configuring ERP resilience pipeline.
/// </summary>
public sealed record ErpResilienceOptions
{
    /// <summary>
    /// Gets the default resilience options.
    /// </summary>
    public static ErpResilienceOptions Default { get; } = new();

    /// <summary>
    /// Gets or sets the operation timeout.
    /// </summary>
    public TimeSpan OperationTimeout { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the maximum retry attempts.
    /// </summary>
    public int MaxRetryAttempts { get; init; } = 3;

    /// <summary>
    /// Gets or sets the base retry delay.
    /// </summary>
    public TimeSpan RetryDelay { get; init; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Gets or sets the circuit breaker failure ratio threshold (0.0 to 1.0).
    /// </summary>
    public double CircuitBreakerFailureRatio { get; init; } = 0.5;

    /// <summary>
    /// Gets or sets the circuit breaker sampling duration.
    /// </summary>
    public TimeSpan CircuitBreakerSamplingDuration { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the minimum throughput before circuit breaker activates.
    /// </summary>
    public int CircuitBreakerMinimumThroughput { get; init; } = 10;

    /// <summary>
    /// Gets or sets the circuit breaker break duration.
    /// </summary>
    public TimeSpan CircuitBreakerBreakDuration { get; init; } = TimeSpan.FromMinutes(1);
}

/// <summary>
/// Exception thrown when an ERP connection fails.
/// </summary>
public class ErpConnectionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErpConnectionException"/> class.
    /// </summary>
    public ErpConnectionException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErpConnectionException"/> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    public ErpConnectionException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErpConnectionException"/> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="innerException">Inner exception.</param>
    public ErpConnectionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown for transient ERP errors that can be retried.
/// </summary>
public class ErpTransientException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErpTransientException"/> class.
    /// </summary>
    public ErpTransientException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErpTransientException"/> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    public ErpTransientException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErpTransientException"/> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="innerException">Inner exception.</param>
    public ErpTransientException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
