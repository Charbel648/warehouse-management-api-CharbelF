# Exercise 03 Raw AI Output — Unit Test Generation

## AI Suggested Test Suite

Target:
RequestTimingMiddleware

Suggested tests:
1. Logs method and request path when request succeeds.
2. Logs response status code.
3. Logs elapsed milliseconds.
4. Does not swallow exceptions thrown by the next middleware.
5. Handles long request paths.
6. Logs server error status codes.

## Human Review

Accepted:
- Testing middleware directly using DefaultHttpContext.
- Mocking ILogger<RequestTimingMiddleware>.
- Verifying logs by checking formatted log text.
- Testing exception propagation instead of hiding exceptions.

Rejected / Adjusted:
- AI originally suggested spinning up a full TestServer for middleware unit tests. Rejected because Exercise 03 is specifically unit-test generation.
- AI suggested asserting exact elapsed millisecond values. Rejected because timing is nondeterministic.
- AI suggested testing database missing entities. Rejected because this selected component is middleware and does not access the database.
- AI suggested parameter format rejection. Rejected because this middleware does not parse route/body parameters.

Final manual strategy:
Use focused unit tests around RequestTimingMiddleware behavior without modifying production code.
