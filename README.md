# Movie API

A small, production-minded ASP.NET Core REST API that demonstrates validated CRUD operations over a thread-safe in-memory movie catalog.

## Features

- Conventional REST endpoints and HTTP status codes
- Request and response DTOs separated from the domain model
- Case-insensitive movie-name lookup and duplicate prevention
- Atomic, thread-safe repository operations
- Automatic RFC 7807 validation responses through ASP.NET Core
- End-to-end API tests and a GitHub Actions CI workflow

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Git

Confirm the SDK is installed:

```bash
dotnet --version
```

## Setup

```bash
git clone <repository-url>
cd MovieAPI
dotnet restore MovieApi.slnx
dotnet build MovieApi.slnx
```

Start the API:

```bash
dotnet run --project MovieApi.csproj
```

The default development profile listens on `http://localhost:5188`. OpenAPI JSON is available in Development at `http://localhost:5188/openapi/v1.json`.

## Commands

| Task | Command |
| --- | --- |
| Restore dependencies | `dotnet restore MovieApi.slnx` |
| Build all projects | `dotnet build MovieApi.slnx --configuration Release` |
| Run all tests | `dotnet test MovieApi.slnx --configuration Release` |
| Run the API | `dotnet run --project MovieApi.csproj` |

## API

The API uses JSON and is rooted at `/api/movies`.

| Method | Endpoint | Success | Description |
| --- | --- | --- | --- |
| `GET` | `/api/movies` | `200 OK` | List every movie. An empty catalog is an empty array. |
| `GET` | `/api/movies?year=1994` | `200 OK` | List movies released in a given year. |
| `GET` | `/api/movies/{name}` | `200 OK` | Find a movie by its case-insensitive name. |
| `POST` | `/api/movies` | `201 Created` | Create a movie. |
| `PUT` | `/api/movies/{name}` | `204 No Content` | Replace a movie, including optionally renaming it. |
| `DELETE` | `/api/movies/{name}` | `204 No Content` | Delete a movie. |

Missing resources return `404 Not Found`. A create or rename that duplicates an existing name returns `409 Conflict`. Invalid requests return `400 Bad Request` with validation details.

### Request schema

| Field | Type | Rules |
| --- | --- | --- |
| `name` | string | Required, non-blank, at most 200 characters |
| `genre` | string | Required, non-blank, at most 100 characters |
| `year` | integer | Between 1888 and 2100 |

Leading and trailing whitespace in names and genres is removed before storage. Names are unique and matched without regard to letter casing.

### Examples

Create a movie:

```bash
curl --request POST http://localhost:5188/api/movies \
  --header "Content-Type: application/json" \
  --data '{"name":"Arrival","genre":"Science Fiction","year":2016}'
```

Read and filter movies:

```bash
curl http://localhost:5188/api/movies/Arrival
curl "http://localhost:5188/api/movies?year=2016"
```

Replace a movie:

```bash
curl --request PUT http://localhost:5188/api/movies/Arrival \
  --header "Content-Type: application/json" \
  --data '{"name":"Arrival","genre":"Science Fiction Drama","year":2016}'
```

Delete a movie:

```bash
curl --request DELETE http://localhost:5188/api/movies/Arrival
```

Additional ready-to-run requests are in [`MovieApi.http`](MovieApi.http).

## Project structure

```text
Controllers/       HTTP routing and response mapping
Dtos/              Public API contracts and validation
Models/            Domain model
Repositories/      Thread-safe in-memory persistence
Services/          Application logic and DTO mapping
tests/             End-to-end API tests
.github/workflows/ Continuous integration
```

## Limitations

- Data is held only in process memory and resets whenever the application restarts.
- The API intentionally has no authentication, authorization, paging, or rate limiting.
- Movie names act as natural identifiers; there is no separate immutable ID.
- The release-year upper bound is 2100 rather than a dynamically maintained catalog rule.
- The repository is suitable for a single application instance only. Multiple instances do not share state.
