# Exercise 08 — Security Review of AI-Generated Upload Code

## Reviewed Code

```csharp
[HttpPost("upload-invoice")]
public async Task<IActionResult> UploadInvoice(IFormFile file)
{
    // WARNING: This AI snippet contains multiple severe security gaps.
    // Analyze for path traversal risks, content manipulation, validation gaps, and unsafe logging.
    var targetFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "invoices");
    var fullPath = Path.Combine(targetFolder, file.FileName);

    using (var stream = new FileStream(fullPath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    return Ok(new { path = fullPath });
}
```

## Executive Summary

The AI-generated upload endpoint is not safe for production.

It directly trusts the client-provided file name, writes the uploaded file to a public web directory, performs no validation, and returns the physical server path in the API response.

The endpoint should be rejected and replaced with a safer upload flow using validated DTOs, application handlers, generated storage keys, object storage abstraction, metadata persistence, authorization policies, and safe response models.

---

# 1. Security Flaws

## 1.1 Trusting `file.FileName`

The code uses:

```csharp
var fullPath = Path.Combine(targetFolder, file.FileName);
```

This is unsafe because `file.FileName` is controlled by the client.

A malicious user could submit file names containing path traversal payloads or unexpected characters.

Examples:

```text
../../appsettings.json
..\..\Program.cs
malicious.aspx
invoice.pdf.exe
```

## Risk

The attacker may write outside the intended folder or overwrite important files if path traversal is not properly blocked.

## Severity

High.

---

## 1.2 Saving Files in `wwwroot`

The code stores invoices inside:

```text
wwwroot/invoices
```

Files under `wwwroot` may be publicly accessible depending on static file configuration.

## Risk

An attacker could upload a malicious file and then access it from the browser.

If executable file types are allowed or server configuration changes, this could become a remote code execution or stored attack vector.

## Severity

High.

---

## 1.3 Returning Physical Server Path

The code returns:

```csharp
return Ok(new { path = fullPath });
```

This leaks internal server structure.

## Risk

Attackers learn directory layout and deployment paths, making future attacks easier.

Example leaked path:

```text
C:\Users\charb\Desktop\warehouse-management-api-CharbelF\Warehouse.Presentation\wwwroot\invoices\file.pdf
```

## Severity

Medium.

---

## 1.4 No Authorization Policy

The endpoint does not show:

```csharp
[Authorize]
```

or a warehouse-specific policy.

## Risk

Unauthenticated or unauthorized users may upload files.

## Severity

High.

---

# 2. Missing Validation

## 2.1 Missing Null Validation

The code never checks:

```csharp
file == null
```

## Risk

A null upload may cause runtime exceptions.

## Correct Behavior

Return a safe validation error, such as 400 Bad Request.

---

## 2.2 Missing Empty File Validation

The code never checks:

```csharp
file.Length <= 0
```

## Risk

Empty files can be uploaded and stored as invalid business records.

---

## 2.3 Missing File Size Limit

There is no file size limit.

## Risk

Attackers can upload very large files and cause:

- disk exhaustion
- memory pressure
- slow request processing
- denial of service

## Correct Behavior

Define a strict maximum size.

Example:

```text
5 MB for invoice PDFs
```

---

## 2.4 Missing Content-Type Allowlist

The code does not verify:

```csharp
file.ContentType
```

## Risk

Attackers can upload scripts, executables, HTML files, or disguised content.

## Correct Behavior

Allow only expected content types.

Example:

```text
application/pdf
image/jpeg
image/png
```

---

## 2.5 Missing Extension Allowlist

The code does not check the extension.

## Risk

Attackers can upload dangerous file types.

Examples:

```text
.exe
.bat
.cmd
.js
.html
.aspx
.php
```

## Correct Behavior

Allow only expected extensions.

Example:

```text
.pdf
.jpg
.jpeg
.png
```

---

## 2.6 Missing Content Inspection

The code only relies on the uploaded file metadata, which can be spoofed.

## Risk

A file can be named `invoice.pdf` and have `application/pdf` while actually containing malicious content.

## Correct Behavior

For sensitive systems, inspect file signatures, scan files, or quarantine before final acceptance.

---

# 3. Bad Exception Handling

## 3.1 No Safe Error Handling

The endpoint does not handle:

- storage failures
- permission failures
- invalid file names
- path errors
- disk space errors
- stream copy failures

## Risk

Unhandled exceptions may return generic 500 errors or leak details depending on middleware configuration.

## Correct Behavior

Let centralized exception middleware return safe messages, but also use expected business exceptions for validation failures.

---

## 3.2 No Cancellation Token

The method does not accept a `CancellationToken`.

## Risk

If the client disconnects, the server may continue processing the upload unnecessarily.

## Better Signature

```csharp
public async Task<IActionResult> UploadInvoice(
    IFormFile file,
    CancellationToken cancellationToken)
```

---

# 4. File Upload Risks

## 4.1 File Overwrite Risk

The code uses:

```csharp
FileMode.Create
```

This can overwrite files with the same name.

## Risk

An attacker may overwrite an existing invoice or important file if the path is predictable.

## Correct Behavior

Generate a unique storage key.

Example:

```text
invoices/{invoiceId}/{Guid.NewGuid()}.pdf
```

---

## 4.2 No Directory Creation Check

The code assumes the folder exists.

## Risk

Upload fails if the folder does not exist.

## Better Behavior

Storage should be handled by an object storage abstraction, not by controller-level physical path logic.

---

## 4.3 No Metadata Persistence

The code stores the file but does not save metadata such as:

- original file name
- generated object key
- content type
- size
- related entity id
- uploaded by
- uploaded at

## Risk

The system cannot safely audit, download, or manage files later.

---

# 5. Path Traversal Risks

## Dangerous Line

```csharp
var fullPath = Path.Combine(targetFolder, file.FileName);
```

`Path.Combine` does not automatically make user input safe.

## Attack Scenario

A malicious filename could attempt to escape the invoices folder.

Example:

```text
..\..\appsettings.json
```

## Correct Mitigation

Do not use client filenames as storage paths.

Use:

- generated object key
- strict extension allowlist
- sanitized original filename stored only as metadata
- final path validation if local disk storage is used

The safest approach in this project is to avoid local direct writes and use the existing object storage abstraction.

---

# 6. Weak Logging

## Missing Logs

The snippet does not log:

- who uploaded the file
- related entity
- file size
- rejected validation reason
- generated file id
- trace id
- storage failure

## Unsafe Logs to Avoid

Do not log:

- full physical server path
- secrets
- bearer tokens
- raw file content

## Correct Logging Strategy

Use structured logs.

Example fields:

```text
UserId
RelatedEntityId
FileCategory
OriginalFileName
ContentType
SizeInBytes
GeneratedFileId
TraceId
ValidationResult
```

---

# 7. Architecture Review

The snippet violates the existing project architecture.

## Problem

The controller performs file storage directly.

## Why This Is Bad

Controllers should remain thin.

They should not directly handle:

- physical paths
- file stream persistence
- storage decisions
- metadata creation
- security validation logic

## Correct Architecture

Use the project architecture:

```text
Controller
→ IMediator
→ UploadInvoiceCommand
→ UploadInvoiceCommandHandler
→ IObjectStorageService
→ WarehouseFile metadata
→ Repository
```

---

# 8. Safer Design Recommendation

## Controller

The controller should only:

- receive the form file
- open the stream
- send a command through IMediator
- return a safe DTO

Example design:

```csharp
[Authorize(Policy = WarehousePolicies.WarehouseAdmin)]
[HttpPost("{invoiceId:guid}/invoice-file")]
[Consumes("multipart/form-data")]
public async Task<ActionResult> UploadInvoice(
    [FromRoute] Guid invoiceId,
    [FromForm] UploadInvoiceRequest request,
    CancellationToken cancellationToken)
{
    await using Stream content = request.File.OpenReadStream();

    var response = await _mediator.Send(new UploadInvoiceCommand
    {
        InvoiceId = invoiceId.ToString(),
        Content = content,
        FileName = request.File.FileName,
        ContentType = request.File.ContentType,
        SizeInBytes = request.File.Length
    }, cancellationToken);

    return Ok(response);
}
```

## Application Handler

The handler should:

- validate null/empty file
- validate size limit
- validate content type
- validate extension
- generate a safe object key
- upload through IObjectStorageService
- persist metadata
- return safe metadata response

## Safe Response

The API should return metadata only:

```json
{
  "fileId": "file-guid",
  "relatedEntityId": "invoice-guid",
  "relatedEntityType": "invoice",
  "fileCategory": "invoice",
  "originalFileName": "invoice.pdf",
  "contentType": "application/pdf",
  "sizeInBytes": 1024,
  "uploadedAt": "2026-08-05T00:00:00Z"
}
```

It should not return:

```json
{
  "path": "C:\\server\\physical\\path\\invoice.pdf"
}
```

---

# 9. Final Threat Findings Table

| Finding | Risk | Severity | Mitigation |
|---|---|---:|---|
| Uses user-provided file name as path | Path traversal / overwrite | High | Generate safe object key |
| Saves into wwwroot | Public file exposure | High | Store outside public root or object storage |
| No size validation | Denial of service | High | Enforce maximum upload size |
| No content-type validation | Malicious file upload | High | Use strict allowlist |
| No extension validation | Dangerous executable upload | High | Use extension allowlist |
| No content inspection | Spoofed file content | Medium | Use file signature validation / scanning |
| Returns physical path | Information disclosure | Medium | Return metadata only |
| No authorization shown | Unauthorized upload | High | Require warehouse admin policy |
| Uses FileMode.Create | Overwrite risk | Medium | Use generated unique key |
| No structured logging | Weak auditability | Medium | Log safe metadata and trace id |
| No cancellation token | Wasted server work | Low | Pass CancellationToken |
| Controller handles storage directly | Architecture violation | Medium | Move logic to application handler |

---

# 10. Final Decision

The AI-generated upload code is rejected.

It contains serious security and architecture problems.

The correct solution should follow the existing Warehouse API upload pattern:

```text
Validated request DTO
→ Thin controller
→ IMediator command
→ Application handler validation
→ Generated object key
→ IObjectStorageService
→ File metadata persistence
→ Safe response DTO
```

This preserves security, testability, and the existing Clean Architecture design.
