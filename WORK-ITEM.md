# Work item to create in your sandbox ADO project

Type: User Story (or Product Backlog Item)

Title: Add input validation to the meter reading endpoint

Description:
The POST /api/meter-readings endpoint accepts any input today. Field devices have sent
negative volumes and blank meter IDs, and those bad readings end up in downstream reports.
Reject invalid readings before they are stored.

Acceptance criteria:
1. MeterId is required and is at most 20 characters.
2. Volume must be zero or greater.
3. Unit must be one of MCF, CCF or M3.
4. ReadAt cannot be in the future.
5. An invalid request returns HTTP 400 with a ProblemDetails body that lists each field error.
6. A valid request still returns HTTP 201, unchanged.
7. Each rule has an automated test.

After you create it, note its ID for the demo (for example 1234).
