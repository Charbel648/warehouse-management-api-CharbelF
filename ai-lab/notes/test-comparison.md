# Exercise 03 — AI Unit Test Generation Comparison

## Selected Component

RequestTimingMiddleware

This component was selected because the project uses MediatR/CQRS handlers rather than traditional ProductService or SupplierService classes. The lab allows selecting an active HTTP custom Middleware, so RequestTimingMiddleware is the best fit without breaking the current architecture.

## AI-Generated Test Strategy

The AI suggested testing:

| Category | AI Suggestion | Accepted? | Notes |
|---|---|---:|---|
| Positive | Verify method, path, status code, elapsed milliseconds, and TraceId are logged | Y | This matches the middleware responsibility. |
| Negative | Verify downstream exceptions are not swallowed | Y | RequestTimingMiddleware is not an exception handler; exception propagation is correct. |
| Negative | Test missing database entities | N | Not applicable because middleware does not use repositories or database access. |
| Negative | Test parameter format rejection | N | Not applicable because this middleware does not parse route parameters. |
| Edge | Very long path should still be logged | Y | Useful edge case for logging safety. |
| Edge | Assert exact elapsed milliseconds | N | Rejected because timing values are nondeterministic and may cause flaky tests. |
| Edge | Timezone boundary configuration | N | Not applicable because this middleware measures elapsed request time, not calendar dates. |

## Manual Testing Strategy

My manual strategy focused on the real responsibility of RequestTimingMiddleware:

1. Standard successful request logs useful observability fields.
2. Response status code is preserved.
3. Elapsed time is logged as a millisecond value, without asserting exact timing.
4. TraceId is included for request correlation.
5. Downstream exceptions are not swallowed.
6. Long request paths and error status codes still produce logs.

## Gaps Found in AI Output

The AI output was useful but too generic in some areas.

Main gaps:
- It suggested database-related negative tests even though the selected target middleware has no database dependency.
- It suggested parameter-format rejection tests, but validation belongs to controllers/model binding, not this middleware.
- It suggested exact timing assertions, which can create flaky tests.
- It did not initially distinguish middleware unit tests from integration tests using TestServer.

## Human Corrections

I manually corrected the AI output by:

- Keeping the tests isolated with DefaultHttpContext.
- Avoiding real server startup.
- Avoiding exact elapsed millisecond assertions.
- Verifying log content instead of exact log formatting.
- Testing only behavior owned by RequestTimingMiddleware.
- Preserving production code without unnecessary refactoring.

## Final Result

The final test suite improves coverage for RequestTimingMiddleware while respecting the existing backend architecture and avoiding unrelated infrastructure concerns.
