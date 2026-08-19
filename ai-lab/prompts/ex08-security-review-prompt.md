# Exercise 08 Prompt History — AI Security Review

## Prompt sent to AI

You are performing a backend security review for an ASP.NET Core Warehouse Management API.

Audit the following AI-generated file-upload endpoint for serious security flaws:

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

Review the code for:
- security flaws
- missing validation
- bad exception handling
- file upload risks
- path traversal risks
- weak logging

Output a formal threat review suitable for ai-lab/notes/security-review.md.

Architecture context:
- The real project uses Clean Architecture.
- Controllers should not contain unsafe file persistence logic.
- File storage should use validated request DTOs, application handlers, object storage abstraction, generated object keys, and safe metadata responses.
- Do not expose internal server paths.
- Do not trust user-provided file names.
