# Card Game Application

A full-stack card game web application built with **.NET 10 Web API**, **Entity Framework Core (SQLite)**, and **React 19 + TypeScript + Vite + Tailwind CSS**.

---

## Prerequisites

Before running the application, ensure you have the following installed on your machine:

- [**.NET 10 SDK**](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (or .NET 10 runtime environment)
- [**Node.js**](https://nodejs.org/) (v18.0.0 or higher) and **npm** (v9.0.0 or higher)
- **Git**

---

## Installation & Setup

1. **Clone the Repository**

   ```bash
   git clone https://github.com/Mo-kon-too-la/Card-Game.git
   cd Card-Game
   ```

2. **Restore .NET Dependencies**

   ```bash
   dotnet restore
   ```

3. **Install Frontend Dependencies (optional)**
   ```bash
   cd CardGame.Client
   npm install
   ```

> **Note on Database Setup:**  
> The application uses **SQLite** (`cardgame.db`). The database schema and any pending migrations are **automatically created** when the backend starts up (`EnsureCreated()` in `Program.cs`). No manual SQL server or migration commands are required!

---

## How to Run the Application

You can run both the API and the Frontend concurrently.

### Running API & Frontend

1. **Start the Backend API:**

   ```bash
   cd CardGame.Server
   dotnet run --launch-profile https
   ```

   - API Endpoint: `https://localhost:7198` / `http://localhost:5242`
   - Swagger OpenAPI Documentation: `https://localhost:7198/api/swagger`
   - If prompted to install self-signed certificate, follow the prompts to proceed

2. **Navigate to the browser of the Frontend Client:**
   - Open your browser and navigate to `https://localhost:56243/`

---

## How to Run the Tests

The solution includes automated unit and integration tests using **xUnit**, **Moq**, and **EF Core In-Memory / SQLite**.

To execute the full test suite:

```bash
dotnet test
```

### Test Coverage

- **Unit Tests (`CardGame.Tests/UnitTests`)**:
  - `DeckServiceTests`: Deck initialization, 104-card distribution (two 52-card decks), and Fisher-Yates shuffling.
  - `ScoringEngineServiceTests`: Card evaluation, hand total sum calculations, suit product calculations, and tie-breaking rules.
  - `GameServiceTests`: Game session orchestration, dealing hands, winner determination, and persistence.
- **Integration Tests (`CardGame.Tests/IntegrationTests`)**:
  - `GamesControllerTests`: End-to-end API HTTP request/response validation for game creation, re-dealing, and paginated game history.

---

## Architecture & Design Decisions

- **Clean Architecture & Layering**:
  - `CardGame.Server`: API controllers, Swagger/OpenAPI documentation, and standardized RFC 7807 `GlobalErrorHandler` middleware.
  - `CardGame.Infrastructure`: EF Core `CardGameDbContext`, SQLite configuration, domain entities (`Game`, `Player`, `Card`, `Score`), database migrations, and domain services (`GameService`, `DeckService`, `ScoringEngineService`).
  - `CardGame.Client`: React single-page application built with TypeScript, Vite, Tailwind CSS, Lucide icons, and light/dark theme support.
  - `CardGame.Tests`: Automated unit and integration test suite.
- **API Endpoints**:
  - **`POST /api/games`**: Starts a new game session. Accepts 6 player names in body, deals 5 cards per player from two 52-card decks, evaluates scores and tie-breakers, persists state, and returns `201 Created` with full game details.
  - **`POST /api/games/{id}/redeal`**: Re-deals cards for an existing game by ID, recalculates hand sums and suit product tie-breakers, updates the database, and returns `200 OK`.
  - **`GET /api/games/{id}`**: Retrieves complete details for a specific game by GUID, returning `200 OK` or `404 Not Found`.
  - **`GET /api/games`**: Returns paginated game history. Supports query parameters `page` (default 1), `pageSize` (default 10), `sortBy` (`date` or `player`), `sortDirection` (`asc` or `desc`), and `filterPlayerName`.
- **Database Schema**:
  - **`Games`**:
    - Columns: `Id` (Guid, Primary Key), `CreatedAtUtc` (DateTime, Indexed), `LastUpdatedAtUtc` (DateTime).
    - Relationships: Has many `Players` (Cascade Delete).
  - **`Players`**:
    - Columns: `Id` (Guid, Primary Key), `GameId` (Guid, Foreign Key), `Name` (string), `SeatNumber` (int, 0-5).
    - Indices: Composite Index on `(GameId, SeatNumber)`.
    - Relationships: Belongs to `Game`, has many `Cards` (Cascade Delete), has one `Score` (Cascade Delete).
  - **`Cards`**:
    - Columns: `Id` (Guid, Primary Key), `PlayerId` (Guid, Foreign Key), `Rank` (string, e.g. "2"-"10", "J", "Q", "K", "A"), `Suit` (string, "♦", "♥", "♠", "♣"), `Value` (int, 2-10, J=11, Q=12, K=13, A=11), `SuitValue` (int, ♦=1, ♥=2, ♠=3, ♣=4), `DeckId` (int, 1 or 2).
    - Indices: Index on `PlayerId`.
    - Relationships: Belongs to `Player`.
  - **`Scores`**:
    - Columns: `Id` (Guid, Primary Key), `PlayerId` (Guid, Foreign Key, Unique), `HandSum` (int), `SuitProduct` (long), `IsTiedForHighestHand` (bool), `IsWinner` (bool).
    - Relationships: Belongs to `Player` (1-to-1).
- **Scoring & Tie-Breaking Engine**:
  - **Hand Score**: Calculated by summing card values (2–10 face value, J=11, Q=12, K=13, A=11).
  - **Suit Tie-Breaker**: If multiple players tie for the highest hand score, the tie-breaker computes the product of suit multipliers (♦ Diamonds = 1, ♥ Hearts = 2, ♠ Spades = 3, ♣ Clubs = 4) for the tied players.
- **Standardized Error Responses**:
  - All unhandled exceptions are caught by `GlobalErrorHandler` middleware and returned as standard `ProblemDetails` (`application/problem+json`) responses.

---

## Assumptions & Trade-Offs

1. **SQLite Database**:
   - **Trade-off:** SQLite uses file-level locking during write operations, making it unsuitable for horizontally scaled multi-node production deployments.
   - **Reasoning:** Chosen for zero-configuration, self-contained local setup without external database dependencies (e.g. PostgreSQL or Docker).
2. **Synchronous Turn Evaluation**:
   - **Trade-off:** Deals 5 cards to all 6 players synchronously in a single request rather than supporting real-time multi-device WebSockets.
   - **Reasoning:** Keeps the focus on robust domain scoring logic, state management, persistence, and client presentation for the assessment scope.

---

## Production Considerations & Future Enhancements

The following enterprise patterns were omitted to maintain a tight focus on the primary assessment scope (domain scoring logic, clean architecture, persistence, and UI experience). In a production-grade system, they would be added as follows:

1. **Structured & Diagnostic Logging (Serilog)**:
   - **Status:** Standard ASP.NET Core `ILogger` console logging is used for error tracking.
   - **Production Approach:** Integration of **Serilog** to enable structured JSON logging. Structured events (capturing `GameId`, `ExecutionTimeMs`, `CorrelationId`) allow logs to be easily indexed, searched, and digested in centralized platforms like Seq, Azure Application Insights, or an ELK stack.

2. **Fault Tolerance & Resilience (Polly / `Microsoft.Extensions.Resilience`)**:
   - **Status:** Omitted because local SQLite interactions and in-memory game evaluations do not involve transient network boundaries.
   - **Production Approach:** Integration of **Polly** (using `Microsoft.Extensions.Resilience`) to configure retry policies with exponential backoff, circuit breakers, and rate-limiting for external service calls or distributed database clusters.

3. **Authentication & Authorization (OpenID Connect / OIDC)**:
   - **Status:** Omitted as user identity management and auth flows were outside the assessment requirements.
   - **Production Approach:** Implementation of **OpenID Connect (OIDC)** and OAuth 2.0 via **Microsoft Identity Platform (Entra ID)** using `Microsoft.AspNetCore.Authentication.OpenIdConnect` / `Microsoft.Identity.Web` to secure API endpoints with JWT Bearer tokens and claims-based authorization.
