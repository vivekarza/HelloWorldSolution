//====================================================================
//README.md
//====================================================================
# Hello World Greeting App

## Setup
- Run the Web Api (`HelloWorldAPI`)
- Run the Console App (`HelloWorldClient`)

## API Endpoints
- `GET /api/greeting?name=Alice` -> returns "Hello Alice"
- `POST /api/greeting` with JSON `{"name": "Bob"}` -> stores and returns "Hello, Bob"

## Features
- **In-Memory Database** using EF Core
- **Client- and Server-Side Caching** (via Dictionary and MemoryCache)
- **Rate Limiting**: Max 5 requests/second (applies globally to all API endpoints)
- **Graceful Error Handling** for API errors and rate-limit violations
- **Upsert Logic**: Updates greeting if name exists, otherwise inserts new
- **Pre-seeded "Hello, World" Greeting**

---
## API Endpoints
### 'GET /api/greeting?name=Alice'
- Returns: `Hello, Alice1`
- If `name` is not provided: defaults to `Hello, World`
- Cached on server for 5 minutes per name

### 'POST /api/greeting'
- Request Body:
```json
{ "name": "Bob" }
```
- Behavior:
  - Adds new greeting if name doesn’t exist
  - Updates message and timestamp if it does
- Response: `Hello, Bob`

---

## Testing
- Unit tests written for:
  - Adding and updating greetings
  - Returning default greeting
- Integration tests verify:
  - Global rate limiting (GET and POST)
  - API responses and persistence logic

To run tests:
```bash
dotnet test HelloWorldAPI.Tests
```

---

## Project Structure
```
HelloWorldSolution/
??? HelloWorldAPI/          # ASP.NET Core Web API
??? HelloWorldClient/       # Console Client App
??? HelloWorldAPI.Tests/    # Unit & Integration Tests
```

---

## Notes
- The application uses a fresh in-memory database for each run.
- Customize caching or replace MemoryCache/EF with production-grade services for real deployment.

---