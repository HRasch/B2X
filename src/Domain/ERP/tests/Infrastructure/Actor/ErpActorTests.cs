// <copyright file="ErpActorTests.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.ERP.Core;
using B2X.ERP.Infrastructure.Actor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Xunit;

namespace B2X.ERP.Tests.Infrastructure.Actor;

/// <summary>
/// Tests for the ErpActor class.
/// </summary>
public sealed class ErpActorTests : IAsyncDisposable
{
    private readonly ILogger<ErpActor> _logger = NullLogger<ErpActor>.Instance;
    private readonly TenantContext _tenant = new()
    {
        TenantId = "test-tenant",
        TenantName = "Test Tenant",
        UserId = "test-user"
    };

    private ErpActor? _actor;

    [Fact]
    public async Task EnqueueAsync_SingleOperation_ExecutesSuccessfully()
    {
        // Arrange
        _actor = new ErpActor(_tenant, _logger);
        var operation = ErpOperation.Create(_tenant, async ct =>
        {
            await Task.Delay(10, ct);
            return 42;
        });

        // Act
        var result = await _actor.EnqueueAsync(operation);

        // Assert
        result.ShouldBe(42);
        _actor.ProcessedOperations.ShouldBe(1);
        _actor.FailedOperations.ShouldBe(0);
    }

    [Fact]
    public async Task EnqueueAsync_MultipleOperations_ExecutesSequentially()
    {
        // Arrange
        _actor = new ErpActor(_tenant, _logger);
        var executionOrder = new List<int>();
        var operations = Enumerable.Range(1, 5).Select(i =>
            ErpOperation.Create(_tenant, async ct =>
            {
                await Task.Delay(10, ct);
                lock (executionOrder)
                {
                    executionOrder.Add(i);
                }
                return i;
            })).ToList();

        // Act - Enqueue all operations in parallel
        var tasks = operations.Select(op => _actor.EnqueueAsync(op));
        var results = await Task.WhenAll(tasks);

        // Assert - Operations executed in order
        results.ShouldBe(new[] { 1, 2, 3, 4, 5 });
        executionOrder.ShouldBe(new[] { 1, 2, 3, 4, 5 });
        _actor.ProcessedOperations.ShouldBe(5);
    }

    [Fact]
    public async Task EnqueueAsync_OperationThrowsException_PropagatesException()
    {
        // Arrange
        _actor = new ErpActor(_tenant, _logger);
        var operation = ErpOperation.Create(_tenant, Task<int> (ct) =>
        {
            throw new InvalidOperationException("Test exception");
        });

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _actor.EnqueueAsync(operation));
        exception.Message.ShouldBe("Test exception");
        _actor.FailedOperations.ShouldBe(1);
    }

    [Fact]
    public async Task EnqueueAsync_OperationTimeout_ThrowsTimeoutException()
    {
        // Arrange
        _actor = new ErpActor(_tenant, _logger);
        var operation = ErpOperation.Create(
            _tenant,
            async ct =>
            {
                await Task.Delay(TimeSpan.FromSeconds(10), ct);
                return 42;
            },
            timeout: TimeSpan.FromMilliseconds(50));

        // Act & Assert
        await Should.ThrowAsync<TimeoutException>(
            async () => await _actor.EnqueueAsync(operation));
    }

    [Fact]
    public async Task EnqueueAsync_WrongTenant_ThrowsInvalidOperationException()
    {
        // Arrange
        _actor = new ErpActor(_tenant, _logger);
        var wrongTenant = new TenantContext { TenantId = "different-tenant", TenantName = "Different Tenant" };
        var operation = ErpOperation.Create(wrongTenant, _ => Task.FromResult(42));

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _actor.EnqueueAsync(operation));
        exception.Message.ShouldContain("does not match");
    }

    [Fact]
    public async Task EnqueueAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        _actor = new ErpActor(_tenant, _logger);
        await _actor.DisposeAsync();

        var operation = ErpOperation.Create(_tenant, _ => Task.FromResult(42));

        // Act & Assert
        await Should.ThrowAsync<ObjectDisposedException>(
            async () => await _actor.EnqueueAsync(operation));
    }

    [Fact]
    public void QueuedOperations_ReturnsCorrectCount()
    {
        // Arrange
        _actor = new ErpActor(_tenant, _logger);

        // Act & Assert
        _actor.QueuedOperations.ShouldBe(0);
    }

    [Fact]
    public void IsReady_NewActor_ReturnsFalse()
    {
        // Arrange
        _actor = new ErpActor(_tenant, _logger);

        // Act & Assert - Not initialized yet
        _actor.IsReady.ShouldBeFalse();
    }

    [Fact]
    public async Task InitializeAsync_SetsIsReady()
    {
        // Arrange
        _actor = new ErpActor(_tenant, _logger);

        // Act
        await _actor.InitializeAsync((t, ct) => Task.CompletedTask);

        // Assert
        _actor.IsReady.ShouldBeTrue();
    }

    [Fact]
    public async Task InitializeAsync_CalledMultipleTimes_OnlyInitializesOnce()
    {
        // Arrange
        _actor = new ErpActor(_tenant, _logger);
        var initCount = 0;

        // Act
        var tasks = Enumerable.Range(0, 5).Select(_ =>
            _actor.InitializeAsync((t, ct) =>
            {
                Interlocked.Increment(ref initCount);
                return Task.CompletedTask;
            }));
        await Task.WhenAll(tasks);

        // Assert
        initCount.ShouldBe(1);
    }

    public async ValueTask DisposeAsync()
    {
        if (_actor != null)
        {
            await _actor.DisposeAsync();
        }
    }
}
