# Exercise 04 Raw AI Output — Integration Testing

## AI Suggested Tests

The AI suggested creating integration tests for:

1. Product creation
   - Assert 201 Created
   - Assert Location header exists
   - Assert returned id/name/sku match request
   - Assert product exists in server-side persistence

2. Product image upload
   - Create product first
   - Upload JPG as multipart form-data
   - Assert 200 OK
   - Assert file metadata response
   - Assert file metadata persisted

3. Product delete/archive
   - Create product first
   - Call DELETE /api/products/{id}
   - Assert 200 OK
   - Assert IsArchived is true
   - Assert product still exists after delete

## Human Review

Accepted:
- Using WebApplicationFactory and HttpClient.
- Reusing existing CustomWebApplicationFactory from the integration test project.
- Verifying both HTTP response and server-side effect.
- Using multipart form-data helper for image upload.

Rejected:
- AI suggested using a real local PostgreSQL connection string. Rejected because tests must be repeatable on any machine and CI.
- AI suggested calling external MinIO for binary storage. Rejected because the integration test should not depend on external services.
- AI suggested only checking status codes. Rejected because the lab requires headers, model properties, and persistent side effects.

Final approach:
Use the existing test infrastructure and test store to verify persistent effects while still going through the real HTTP pipeline.
