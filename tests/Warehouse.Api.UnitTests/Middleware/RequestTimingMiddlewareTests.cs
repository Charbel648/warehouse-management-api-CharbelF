using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Warehouse.Presentation.Middleware;

namespace Warehouse.Api.UnitTests.Middleware;

public class RequestTimingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldLogRequestPath()
    {
        var loggerMock = new Mock<ILogger<RequestTimingMiddleware>>();

        RequestDelegate next = context =>
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            return Task.CompletedTask;
        };

        var middleware = new RequestTimingMiddleware(next, loggerMock.Object);

        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/api/products";
        context.TraceIdentifier = "trace-001";

        await middleware.InvokeAsync(context);

        LogShouldContain(loggerMock, "/api/products");
        LogShouldContain(loggerMock, "GET");
    }

    [Fact]
    public async Task InvokeAsync_ShouldLogResponseStatusCode()
    {
        var loggerMock = new Mock<ILogger<RequestTimingMiddleware>>();

        RequestDelegate next = context =>
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        };

        var middleware = new RequestTimingMiddleware(next, loggerMock.Object);

        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/api/products/missing-id";
        context.TraceIdentifier = "trace-002";

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        LogShouldContain(loggerMock, "responded 404");
    }

    [Fact]
    public async Task InvokeAsync_ShouldLogElapsedMilliseconds()
    {
        var loggerMock = new Mock<ILogger<RequestTimingMiddleware>>();

        RequestDelegate next = async context =>
        {
            await Task.Delay(5);
            context.Response.StatusCode = StatusCodes.Status201Created;
        };

        var middleware = new RequestTimingMiddleware(next, loggerMock.Object);

        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/api/products";
        context.TraceIdentifier = "trace-003";

        await middleware.InvokeAsync(context);

        LogShouldContain(loggerMock, "in");
        LogShouldContain(loggerMock, "ms");
        LogShouldContain(loggerMock, "trace-003");
    }
    private static void LogShouldContain(
        Mock<ILogger<RequestTimingMiddleware>> loggerMock,
        string expectedText)
    {
        loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((value, _) =>
                    value.ToString() != null &&
                    value.ToString()!.Contains(expectedText)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}

