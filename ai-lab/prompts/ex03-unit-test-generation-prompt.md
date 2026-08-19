# Exercise 03 Prompt History — AI Unit Test Generation

## Selected Target Component

RequestTimingMiddleware

Reason:
The lab allows selecting an active piece of HTTP custom Middleware.
The current project does not use ProductService or SupplierService classes for the main product/supplier logic; it uses MediatR/CQRS handlers.
Therefore, testing RequestTimingMiddleware preserves the existing architecture.

## Prompt sent to AI

You are assisting with an ASP.NET Core Warehouse Management API.

Target component:
RequestTimingMiddleware

Current middleware behavior:
- Starts a Stopwatch.
- Registers an X-Response-Time header using Response.OnStarting.
- Calls the next middleware delegate.
- Stops the stopwatch.
- Logs HTTP method, request path, response status code, elapsed milliseconds, and TraceId.

Generate xUnit tests using:
- Moq
- FluentAssertions
- DefaultHttpContext
- ILogger<RequestTimingMiddleware>

Required test categories:
1. Positive tests:
   - Standard execution path with valid method, path, TraceId, and successful status code.
   - Verify method, path, status code, elapsed ms, and TraceId are logged.

2. Negative tests:
   - Downstream middleware throws an exception.
   - Verify the exception is not swallowed by RequestTimingMiddleware.

3. Edge cases:
   - Very long request path should still be logged.
   - Error status code should still be logged.

Rules:
- Do not use real web server.
- Do not call external services.
- Do not use .Result or .Wait().
- Do not change production middleware unless tests reveal a real bug.
- Keep tests isolated and deterministic.
