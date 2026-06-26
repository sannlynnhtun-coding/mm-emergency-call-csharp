# MM Emergency Call API

MM Emergency Call is an ASP.NET Core Web API for managing emergency requests,
emergency service providers, users, locations, and admin dashboard reporting.
The API is organized by menu-oriented feature boundaries so the backend modules
match the navigation areas used by the application.

## Project Overview

The solution is split into small projects with clear ownership:

| Project | Purpose |
| --- | --- |
| `MMEmergencyCall.Api` | API host, dependency injection, Swagger, global error handling, and request pipeline. |
| `MMEmergencyCall.Domain.Admin` | Admin menu features such as Auth, Dashboard, Users, Townships, State Regions, Emergency Requests, and Emergency Services. |
| `MMEmergencyCall.Domain.Client` | Client menu features such as Auth, Profile, Emergency Requests, and Emergency Services. |
| `MMEmergencyCall.Database` | EF Core `AppDbContext`, database entities, and Dapper infrastructure. |
| `MMEmergencyCall.Shared` | Shared result pattern, enums, encryption helpers, and common utilities. |
| `MMEmergencyCall.ResultPattern.Tests` | xUnit service-level tests for all feature services and result mapping. |

## Architecture

The codebase uses Menu-Oriented Feature Architecture:

```text
Features
|-- Auth
|-- Dashboard
|-- EmergencyRequests
|-- EmergencyServices
|-- Profile
|-- StateRegions
|-- Townships
`-- Users
```

Each feature keeps its controller, service, request models, response models, and
item models close together. Business logic stays inside the owning feature unless
it is shared infrastructure.

All APIs return the shared `Result<T>` response pattern:

```json
{
  "isSuccess": true,
  "isError": false,
  "data": {},
  "message": "Success"
}
```

HTTP status mapping is centralized through the base controller:

| Result type | HTTP status |
| --- | --- |
| `Success` | `200 OK` |
| `BadRequest`, `ValidationError`, `InvalidData`, `Error` | `400 Bad Request` |
| `Unauthorized` | `401 Unauthorized` |
| `NotFound` | `404 Not Found` |
| `DuplicateRecord` | `409 Conflict` |
| `SystemError` | `500 Internal Server Error` |

Global invalid model state and unhandled exceptions also return `Result<T>`
shaped responses.

## Main Features

Admin features:

- Auth: register, sign in, refresh token, sign out.
- Dashboard: request summaries, active totals, top request/service reports.
- Users: create, list, detail, update, and delete status.
- State Regions and Townships: CRUD and paginated township listing.
- Emergency Requests: list and status changes.
- Emergency Services: create, list, approve/reject, update, and delete status.

Client features:

- Auth: register with OTP email, verify, sign in, and logout.
- Profile: detail and deactivate.
- Emergency Requests: submit, list, detail, history, and status update.
- Emergency Services: list/search, detail, create, update, delete, service types,
  and distance-based search.

## Request Workflow

Typical API flow:

```text
HTTP request
  -> Controller
  -> Feature service
  -> EF Core / Dapper data access
  -> Result<T>
  -> Base controller HTTP mapping
  -> JSON response
```

Validation and business failures are returned as `Result<T>` errors instead of
raw strings or direct framework error bodies. Unexpected exceptions are handled
by the global exception handler and returned as `SystemError`.

## Development Workflow

1. Restore packages:

   ```bash
   dotnet restore MMEmergencyCall.sln
   ```

2. Configure the database connection.

   Use local development configuration, user secrets, or environment variables.
   Do not commit real database or SMTP credentials.

   Expected connection string key:

   ```text
   ConnectionStrings:DbConnection
   ```

3. Run the API:

   ```bash
   dotnet run --project MMEmergencyCall.Api
   ```

4. Open Swagger:

   ```text
   /swagger
   ```

## Database Workflow

The project uses EF Core with SQL Server for application runtime and SQLite
in-memory for service tests.

To re-scaffold the database context from SQL Server, use a local connection
string with your own credentials:

```bash
dotnet ef dbcontext scaffold "<local-connection-string>" Microsoft.EntityFrameworkCore.SqlServer -o AppDbContextModels -c AppDbContext -f
```

Review scaffolded changes before committing because generated entity changes can
affect API behavior and tests.

## Testing Workflow

Run all tests:

```bash
dotnet test MMEmergencyCall.sln --no-restore
```

Run build verification:

```bash
dotnet build MMEmergencyCall.sln --no-restore
```

The test project covers:

- `Result<T>` factories and base controller HTTP mapping.
- All Admin and Client feature services returning `Result<T>`.
- EF-backed services using SQLite in-memory.
- SMTP and dashboard stored-procedure paths through small fakes.

Target framework:

- The solution targets `.NET 10` / `net10.0`.

## Adding Or Changing A Feature

1. Put code under the matching menu feature folder.
2. Keep one controller and one service per menu feature.
3. Add or update request/response/item models inside the same feature.
4. Return `Result<T>` from service methods.
5. Use the base controller `Execute` method for HTTP responses.
6. Add service-level xUnit coverage for success and the main failure path.
7. Run test and build verification before commit.
