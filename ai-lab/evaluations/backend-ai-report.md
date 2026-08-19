# Backend AI Report — Session 10

## Session

Session 10 — Apply Generative AI in Backend Engineering Workflow

## Repository

warehouse-management-api

## Branch

session-10-generative-ai-backend

## Goal

The goal of this session was to use generative AI for backend engineering tasks while keeping human review, architecture control, testing, and security verification as the final authority.

The work focused on:

- AI-assisted API feature development
- AI debugging
- AI unit test generation
- AI integration testing
- AI refactoring
- AI documentation
- AI architecture design
- AI security review
- prompt comparison
- AI risk analysis

---

# 1. Best Prompt

The best prompt was the corrected architecture-aware backend prompt:

```text
Create an ASP.NET Core endpoint for warehouse products using the existing Clean Architecture and MediatR/CQRS pattern.

Requirements:
- Controller must call IMediator only.
- Do not inject DbContext into the controller.
- Do not create ProductService if the project already uses handlers.
- Add request DTO and response DTO if needed.
- Use DataAnnotations or existing validation style.
- Add an application command/query and handler.
- Use repository interfaces from the Domain layer.
- Keep EF Core inside Infrastructure repositories only.
- Add unit tests with mocked dependencies.
- Add WebApplicationFactory integration tests.
- Assert status codes, headers, response properties, and server-side effects.
- Preserve authorization policies.
- Use async/await only.
- Do not use .Result or .Wait().
```

## Why This Was the Best Prompt

This prompt was effective because it gave the AI exact project context.

It reduced hallucinated architecture and forced the AI to respect:

- thin controllers
- IMediator usage
- MediatR/CQRS handlers
- repository boundaries
- DTO usage
- authorization policies
- async/await safety
- test expectations

---

# 2. Best Generated Code

The best generated and manually accepted code was the Exercise 01 endpoint:

```text
GET /api/products/expiring-soon
```

## Final Architecture

```text
ProductsController
→ IMediator
→ GetExpiringSoonProductsQuery
→ GetExpiringSoonProductsHandler
→ IProductRepository
→ ProductRepository
```

## Why This Was the Best Code

It was accepted because:

- the controller stayed thin
- no DbContext was added to Presentation
- no random ProductService was created
- the application handler contained use-case logic
- the repository performed EF Core filtering
- archived products were excluded
- already expired products were excluded
- unit tests verified handler behavior
- the feature matched the existing backend architecture

---

# 3. Incorrect Generated Code / Hallucinations Discovered

During the session, several AI outputs had to be rejected or corrected.

## 3.1 ProductService Hallucination

AI repeatedly suggested creating a `ProductService`.

Rejected because the project uses:

```text
Controller
→ IMediator
→ Command / Query Handler
→ Repository Interface
→ Infrastructure Repository
```

Creating a ProductService would violate the existing structure.

---

## 3.2 DbContext in Controller

Some AI outputs suggested injecting `WarehouseDbContext` directly into controllers.

Rejected because controllers must not access infrastructure directly.

Correct pattern:

```text
Controller
→ IMediator
→ Handler
→ Repository Interface
```

---

## 3.3 Unsafe File Upload

The AI-generated upload snippet used:

```csharp
Path.Combine(targetFolder, file.FileName)
```

Rejected because it introduced:

- path traversal risk
- public file exposure
- missing validation
- overwrite risk
- server path disclosure

Correct approach:

```text
Validated DTO
→ IMediator command
→ Application handler
→ generated object key
→ object storage abstraction
→ metadata response
```

---

## 3.4 Direct Notification Service Calls

In the shipment module design, AI suggested direct notification calls.

Rejected because it creates synchronous distributed coupling.

Correct approach:

```text
Shipment event
→ RabbitMQ
→ Notification Service
```

---

## 3.5 Real Infrastructure in Integration Tests

Some AI output suggested using real PostgreSQL, MinIO, RabbitMQ, or Firebase during integration tests.

Rejected because tests must be repeatable and isolated.

Correct approach:

- WebApplicationFactory
- fake authentication
- in-memory test store
- fake object storage
- no-op event publisher

---

## 3.6 Exact Timing Assertions

For RequestTimingMiddleware tests, AI suggested exact elapsed millisecond checks.

Rejected because timing tests can become flaky.

Correct approach:

- verify that elapsed time is logged
- do not assert exact milliseconds

---

# 4. Human Refactoring Steps Required

Human review was required to:

1. Replace service-style AI designs with MediatR/CQRS handlers.
2. Keep controller actions thin.
3. Ensure no DbContext was introduced into Presentation.
4. Ensure repository methods stayed inside Infrastructure.
5. Add `AsNoTracking()` only to safe read-only queries.
6. Avoid `AsNoTracking()` on tracked mutation flows.
7. Fix expiring-soon filtering to exclude already expired products.
8. Reject insecure file upload code.
9. Preserve Firebase authorization policies.
10. Preserve RabbitMQ async messaging boundaries.
11. Review generated tests for flakiness.
12. Run the full test suite after changes.

---

# 5. Backend Engineering Lessons Learned

## Lesson 1 — AI Needs Architecture Context

Generic prompts can produce code that compiles but violates project architecture.

The prompt must explain the existing structure before asking for code.

---

## Lesson 2 — Compiling Code Is Not Enough

Backend code must be checked for:

- business correctness
- security
- performance
- authorization
- testability
- maintainability

---

## Lesson 3 — Security Review Is Mandatory

AI-generated upload code can be dangerous even when it looks simple.

File uploads require:

- size validation
- extension allowlist
- content type allowlist
- generated object keys
- metadata-only response
- no physical path exposure
- authorization
- logging
- object storage abstraction

---

## Lesson 4 — Tests Must Match the Component

AI sometimes suggests irrelevant tests.

For RequestTimingMiddleware, database tests were rejected because middleware does not use the database.

---

## Lesson 5 — Human Review Owns the Final Decision

AI can accelerate drafting, but the developer must decide what is safe to merge.

---

# 6. Exercise 10 — AI Risk Review Table

| AI Risk | Possible Impact | Mitigation | Human Verification |
|---|---|---|---|
| Wrong validation | Invalid data can enter the system, business rules can be bypassed, archived products may be updated, negative quantity or invalid prices may be accepted | Use request DTO validation, domain guards, handler-level checks, and automated tests for valid and invalid cases | Review DTO attributes, domain methods, and unit tests. Run multi-scenario tests for bad input, boundary input, and valid input |
| Insecure file upload | Path traversal, public file exposure, overwrite attacks, malicious file storage, information disclosure, possible remote code execution depending on hosting configuration | Use file size limits, extension allowlist, content-type allowlist, generated object keys, object storage abstraction, metadata-only responses, and authorization policies | Perform senior developer security review, static analysis, and upload abuse tests with dangerous filenames, large files, wrong extensions, and spoofed content types |
| Incorrect EF logic | Wrong records returned, archived records exposed, expired records included, N+1 queries, slow queries, tracking memory overhead, possible data corruption in update flows | Keep EF Core inside repositories, use `AsNoTracking()` only for read-only queries, review filters carefully, avoid unnecessary Include, avoid per-row queries | Review EF queries, inspect generated logic, run unit/integration tests, check for N+1 patterns, and verify no `.Result` or `.Wait()` |
| Missing auth mappings | Unauthorized users may access admin endpoints, readers may mutate data, object-level security may be broken, protected file routes may be exposed | Use explicit authorization policies on controllers/actions, verify Firebase claim mapping, test 401/403 behavior, avoid anonymous sensitive endpoints | Run integration security tests for unauthenticated, reader, and admin users. Review policy names and claim mappings manually |

---

# 7. Verification Performed

The following verification commands should be run before the final PR:

```powershell
dotnet test .\tests\Warehouse.Api.UnitTests\Warehouse.Api.UnitTests.csproj
dotnet test .\tests\Warehouse.Api.IntegrationTests\Warehouse.Api.IntegrationTests.csproj
dotnet test .\warehouse_management.sln
git status --short
```

Expected result:

```text
All tests pass.
Working tree is clean.
```

---

# 8. Final Session 10 Checklist

| Checklist Item | Status | Notes |
|---|---:|---|
| Correct branch used | Done | session-10-generative-ai-backend |
| AI prompts saved | Done | Stored under ai-lab/prompts |
| Generated outputs saved | Done | Stored under ai-lab/generated-code |
| Evaluations saved | Done | Stored under ai-lab/evaluations |
| Notes saved | Done | Stored under ai-lab/notes |
| Architecture preserved | Done | Controller → IMediator → Handler → Repository |
| DTO usage reviewed | Done | DTOs used or proposed where appropriate |
| Tests added/reviewed | Done | Unit and integration tests added in earlier exercises |
| Security review completed | Done | security-review.md |
| Prompt comparison completed | Done | prompt-comparison.md |
| Risk review completed | Done | This report |
| Human corrections documented | Done | ProductService, DbContext, uploads, notification coupling |
| Final tests required before PR | Pending local run | Run dotnet test before pushing |

---

# 9. Final Conclusion

Generative AI was useful for drafting backend code, tests, documentation, and architecture ideas.

However, the AI output required human correction to preserve:

- Clean Architecture
- MediatR/CQRS usage
- authorization policies
- repository boundaries
- secure file handling
- async messaging boundaries
- test reliability
- EF Core performance safety

The final accepted work is the result of AI assistance combined with manual backend engineering review.
