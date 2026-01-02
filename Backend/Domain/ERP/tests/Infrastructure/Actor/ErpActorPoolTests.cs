// <copyright file="ErpActorPoolTests.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2Connect.ERP.Core;
using B2Connect.ERP.Infrastructure.Actor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Xunit;

namespace B2Connect.ERP.Tests.Infrastructure.Actor;

/// <summary>
/// Tests for the ErpActorPool class.
/// </summary>
public sealed class ErpActorPoolTests : IAsyncDisposable
{
    private readonly ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;
    private ErpActorPool? _pool;

    [Fact]
    public async Task GetOrCreateActorAsync_NewTenant_CreatesActor()
    {
        // Arrange
        _pool = new ErpActorPool(_loggerFactory);
        var tenant = new TenantContext { TenantId = "tenant-1", TenantName = "Tenant 1" };

        // Act
        var actor = await _pool.GetOrCreateActorAsync(tenant);

        // Assert
        actor.ShouldNotBeNull();
        actor.Tenant.TenantId.ShouldBe("tenant-1");
        _pool.ActiveActorCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetOrCreateActorAsync_SameTenant_ReturnsSameActor()
    {
        // Arrange
        _pool = new ErpActorPool(_loggerFactory);
        var tenant = new TenantContext { TenantId = "tenant-1", TenantName = "Tenant 1" };

        // Act
        var actor1 = await _pool.GetOrCreateActorAsync(tenant);
        var actor2 = await _pool.GetOrCreateActorAsync(tenant);

        // Assert
        actor1.ShouldBeSameAs(actor2);
        _pool.ActiveActorCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetOrCreateActorAsync_DifferentTenants_CreatesDifferentActors()
    {
        // Arrange
        _pool = new ErpActorPool(_loggerFactory);
        var tenant1 = new TenantContext { TenantId = "tenant-1", TenantName = "Tenant 1" };
        var tenant2 = new TenantContext { TenantId = "tenant-2", TenantName = "Tenant 2" };

        // Act
        var actor1 = await _pool.GetOrCreateActorAsync(tenant1);
        var actor2 = await _pool.GetOrCreateActorAsync(tenant2);

        // Assert
        actor1.ShouldNotBeSameAs(actor2);
        _pool.ActiveActorCount.ShouldBe(2);
    }

    [Fact]
    public async Task ActiveTenants_ReturnsAllTenantIds()
    {
        // Arrange
        _pool = new ErpActorPool(_loggerFactory);
        await _pool.GetOrCreateActorAsync(new TenantContext { TenantId = "tenant-1", TenantName = "Tenant 1" });
        await _pool.GetOrCreateActorAsync(new TenantContext { TenantId = "tenant-2", TenantName = "Tenant 2" });
        await _pool.GetOrCreateActorAsync(new TenantContext { TenantId = "tenant-3", TenantName = "Tenant 3" });

        // Act
        var tenants = _pool.ActiveTenants;

        // Assert
        tenants.Count.ShouldBe(3);
        tenants.ShouldContain("tenant-1");
        tenants.ShouldContain("tenant-2");
        tenants.ShouldContain("tenant-3");
    }

    [Fact]
    public async Task RemoveActorAsync_ExistingTenant_RemovesActor()
    {
        // Arrange
        _pool = new ErpActorPool(_loggerFactory);
        var tenant = new TenantContext { TenantId = "tenant-1", TenantName = "Tenant 1" };
        await _pool.GetOrCreateActorAsync(tenant);

        // Act
        await _pool.RemoveActorAsync("tenant-1");

        // Assert
        _pool.ActiveActorCount.ShouldBe(0);
        _pool.ActiveTenants.ShouldBeEmpty();
    }

    [Fact]
    public async Task RemoveActorAsync_NonExistentTenant_DoesNothing()
    {
        // Arrange
        _pool = new ErpActorPool(_loggerFactory);

        // Act
        await _pool.RemoveActorAsync("non-existent");

        // Assert
        _pool.ActiveActorCount.ShouldBe(0);
    }

    [Fact]
    public async Task ExecuteAsync_RoutesToCorrectActor()
    {
        // Arrange
        _pool = new ErpActorPool(_loggerFactory);
        var tenant1 = new TenantContext { TenantId = "tenant-1", TenantName = "Tenant 1" };
        var tenant2 = new TenantContext { TenantId = "tenant-2", TenantName = "Tenant 2" };

        var operation1 = ErpOperation.Create(tenant1, _ => Task.FromResult("tenant-1-result"));
        var operation2 = ErpOperation.Create(tenant2, _ => Task.FromResult("tenant-2-result"));

        // Act
        var result1 = await _pool.ExecuteAsync(operation1);
        var result2 = await _pool.ExecuteAsync(operation2);

        // Assert
        result1.ShouldBe("tenant-1-result");
        result2.ShouldBe("tenant-2-result");
    }

    [Fact]
    public async Task GetStatistics_ReturnsActorStats()
    {
        // Arrange
        _pool = new ErpActorPool(_loggerFactory);
        var tenant = new TenantContext { TenantId = "tenant-1", TenantName = "Tenant 1" };
        var actor = await _pool.GetOrCreateActorAsync(tenant);
        await actor.InitializeAsync((t, ct) => Task.CompletedTask);

        // Execute some operations
        await _pool.ExecuteAsync(ErpOperation.Create(tenant, _ => Task.FromResult(1)));
        await _pool.ExecuteAsync(ErpOperation.Create(tenant, _ => Task.FromResult(2)));

        // Act
        var stats = _pool.GetStatistics();

        // Assert
        stats.ShouldContainKey("tenant-1");
        stats["tenant-1"].TenantId.ShouldBe("tenant-1");
        stats["tenant-1"].ProcessedOperations.ShouldBeGreaterThanOrEqualTo(2);
        stats["tenant-1"].IsReady.ShouldBeTrue();
    }

    [Fact]
    public async Task GetOrCreateActorAsync_ConcurrentAccess_OnlyCreatesOneActor()
    {
        // Arrange
        _pool = new ErpActorPool(_loggerFactory);
        var tenant = new TenantContext { TenantId = "tenant-1", TenantName = "Tenant 1" };

        // Act - Create actors concurrently
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => _pool.GetOrCreateActorAsync(tenant));
        var actors = await Task.WhenAll(tasks);

        // Assert - All should be the same actor
        foreach (var actor in actors)
        {
            actor.ShouldBeSameAs(actors[0]);
        }

        _pool.ActiveActorCount.ShouldBe(1);
    }

    [Fact]
    public async Task DisposeAsync_DisposesAllActors()
    {
        // Arrange
        _pool = new ErpActorPool(_loggerFactory);
        await _pool.GetOrCreateActorAsync(new TenantContext { TenantId = "tenant-1", TenantName = "Tenant 1" });
        await _pool.GetOrCreateActorAsync(new TenantContext { TenantId = "tenant-2", TenantName = "Tenant 2" });

        // Act
        await _pool.DisposeAsync();

        // Assert
        _pool.ActiveActorCount.ShouldBe(0);
    }

    public async ValueTask DisposeAsync()
    {
        if (_pool != null)
        {
            await _pool.DisposeAsync();
        }
    }
}
