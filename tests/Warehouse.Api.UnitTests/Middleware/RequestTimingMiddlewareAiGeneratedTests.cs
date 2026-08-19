using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Warehouse.Presentation.Middleware;

namespace Warehouse.Api.UnitTests.Middleware;

public class RequestTimingMiddlewareAiGeneratedTests
{
    [Fact]
    public async Task InvokeAsync_WhenRequestSucceeds_ShouldLogMethodPathStatusElapsedAndTraceId()
    {
        var loggerMock = new Mock<ILogger<RequestTimingMiddleware>>();

        RequestDelegate next = context =>
        {
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return Task.CompletedTask;
        };

        var middleware = new RequestTimingMiddleware(next, loggerMock.Object);

        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Put;
        context.Request.Path = "/api/products/11111111-1111-1111-1111-111111111111/quantity";
        context.TraceIdentifier = "trace-positive-001";

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        LogShouldContain(loggerMock, "PUT");
        LogShouldContain(loggerMock, "/api/products/11111111-1111-1111-1111-111111111111/quantity");
        LogShouldContain(loggerMock, "204");
        LogShouldContain(loggerMock, "ms");
        LogShouldContain(loggerMock, "trace-positive-001");
    }

    [Fact]
    public async Task InvokeAsync_WhenNextMiddlewareThrows_ShouldNotSwallowException()
    {
        var loggerMock = new Mock<ILogger<RequestTimingMiddleware>>();

        RequestDelegate next = _ =>
            throw new InvalidOperationException("Simulated downstream failure");

        var middleware = new RequestTimingMiddleware(next, loggerMock.Object);

        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/api/products";
        context.TraceIdentifier = "trace-negative-001";

        Func<Task> act = async () => await middleware.InvokeAsync(context);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Simulated downstream failure");
    }

    [Fact]
    public async Task InvokeAsync_WithVeryLongRequestPath_ShouldStillLogPath()
    {
        var loggerMock = new Mock<ILogger<RequestTimingMiddleware>>();

        RequestDelegate next = context =>
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            return Task.CompletedTask;
        };

        var middleware = new RequestTimingMiddleware(next, loggerMock.Object);

        string longPath = "/api/products/" + new string('a', 2048);

        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = longPath;
        context.TraceIdentifier = "trace-edge-long-path";

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);

        LogShouldContain(loggerMock, longPath);
        LogShouldContain(loggerMock, "trace-edge-long-path");
    }

    [Fact]
    public async Task InvokeAsync_WithServerErrorStatusCode_ShouldLogErrorStatus()
    {
        var loggerMock = new Mock<ILogger<RequestTimingMiddleware>>();

        RequestDelegate next = context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return Task.CompletedTask;
        };

        var middleware = new RequestTimingMiddleware(next, loggerMock.Object);

        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/api/products";
        context.TraceIdentifier = "trace-edge-500";

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        LogShouldContain(loggerMock, "POST");
        LogShouldContain(loggerMock, "/api/products");
        LogShouldContain(loggerMock, "500");
        LogShouldContain(loggerMock, "trace-edge-500");
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
