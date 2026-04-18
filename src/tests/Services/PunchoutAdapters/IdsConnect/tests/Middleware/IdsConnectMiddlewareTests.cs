// <copyright file="IdsConnectMiddlewareTests.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.IdsConnectAdapter.Middleware;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace B2X.IdsConnectAdapter.Tests.Middleware;

/// <summary>
/// Unit tests for the IdsConnectMiddleware.
/// </summary>
public sealed class IdsConnectMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_IdsApiPath_SetsXmlContentType()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/ids/catalog";
        var nextCalled = false;

        var middleware = new IdsConnectMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("application/xml", context.Response.ContentType);
    }

    [Fact]
    public async Task InvokeAsync_NonIdsPath_DoesNotSetXmlContentType()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/products";

        var middleware = new IdsConnectMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Null(context.Response.ContentType);
    }

    [Fact]
    public async Task InvokeAsync_AnyPath_AddsIdsHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/something";

        var middleware = new IdsConnectMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal("2.5", context.Response.Headers["X-IDS-Version"]);
        Assert.Equal("B2X", context.Response.Headers["X-IDS-Provider"]);
    }

    [Fact]
    public async Task InvokeAsync_CallsNextDelegate()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var nextCallCount = 0;

        var middleware = new IdsConnectMiddleware(_ =>
        {
            nextCallCount++;
            return Task.CompletedTask;
        });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(1, nextCallCount);
    }
}
