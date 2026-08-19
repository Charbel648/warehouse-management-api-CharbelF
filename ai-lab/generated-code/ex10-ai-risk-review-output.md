# Exercise 10 Raw AI Output — Backend AI Risk Review

## AI Risk Review Summary

The AI identified four major backend risks:

1. Wrong validation can allow invalid business state.
2. Insecure file upload can create severe security vulnerabilities.
3. Incorrect EF logic can cause wrong data, performance issues, or data corruption.
4. Missing auth mappings can expose protected actions to unauthorized users.

## Human Review

Accepted:
- The risks are realistic for backend AI-generated code.
- Each risk requires automated tests and human code review.
- Security and authorization risks require special attention.
- EF Core logic must be reviewed for performance and correctness.

Rejected:
- The AI suggested relying only on unit tests. This is not enough.
- The AI suggested accepting code if it compiles. This is not enough.
- The AI suggested generic validation without domain rules. This is not enough.
- The AI suggested manual testing only. This is not enough.

Final decision:
AI-generated backend code must pass human review, automated tests, architecture review, security review, and regression verification before it is accepted.
