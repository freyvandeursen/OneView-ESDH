# OneView - ESDH: Backend

The ASP.NET Core backend for **OneView**, an ESDH-inspired prototype that brings customer information and related documents into one workspace. Its current scope covers customer management and document access; structured case processing is not yet implemented.

OneView is a group project developed by **six people in collaboration with Vitani**. This backend is part of the team's shared work.

## Start the application locally

Use PowerShell and start **each terminal in the repository root**, which contains both `oneview-backend-main` and `oneview-frontend-main`. The commands below use paths relative to that root.

You need Docker Desktop running Linux containers, a .NET SDK with the .NET 9 ASP.NET Core runtime, Node.js 22.12+ or 24, and pnpm 10. Before the first run, follow the [database and secrets setup](../README.md#1-start-sql-server-and-redis) in the main guide. Your JWT key and database connection string must be configured privately; runtime secrets are not included in the public repository.

If http://127.0.0.1:5173/ already opens and http://localhost:8080/Health reports `Connected`, the application is already running. Use those services rather than starting a second copy.

### 1. Start the database and backend

Open a PowerShell terminal and run:

```powershell
# Start the existing SQL Server and Redis containers.
docker start oneview-db oneview-redis

# Load the saved User Secrets and listen on the port expected by the frontend.
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project .\oneview-backend-main\OneView-API\OneView-API.csproj --no-launch-profile --urls http://localhost:8080
```

Wait for SQL Server to finish starting if the first API attempt cannot connect, then rerun the `dotnet run` command. Keep this terminal open while using the application.

The API applies its SQL schema updates automatically at startup. **No `dotnet ef database update` command or Docker image migration is needed to update the schema.** Preserve the existing database container: its data is stored inside the container rather than a mounted volume.

### 2. Start the frontend

Open a **second** PowerShell terminal in the repository root and run:

```powershell
Set-Location .\oneview-frontend-main

# Needed on the first run, or after dependency/lockfile changes.
pnpm install --frozen-lockfile

pnpm dev --host 127.0.0.1 --port 5173 --strictPort
```

Keep this terminal open too. Vite forwards frontend `/api` requests to the backend on port **8080**, so keep that backend port when following this guide.

### 3. Open the app and sign in

Open **http://127.0.0.1:5173/login**.

| Username | Password | Role |
| --- | --- | --- |
| `admin` | `adminpass` | Administrator |
| `user` | `pass` | User |

These are local demo accounts. Successful login opens the customer list.

Useful links:

- [Frontend](http://127.0.0.1:5173/)
- [API and database health](http://localhost:8080/Health) — should report `Healthy` and `Connected`.
- [Swagger API documentation](http://localhost:8080/swagger)

To check connectivity from another PowerShell terminal:

```powershell
Invoke-RestMethod http://localhost:8080/Health
Invoke-RestMethod http://127.0.0.1:5173/api/Health
docker exec oneview-redis redis-cli PING
```

Redis should return `PONG`.

### Secrets, first-time setup, and common problems

- **Configure secrets once.** The API loads `Jwt:Key` and the SQL connection string from .NET User Secrets when `ASPNETCORE_ENVIRONMENT` is `Development`. Skip regenerating secrets during routine restarts. There is no frontend authentication secret to set.
- **First run or missing `Jwt:Key`:** follow the [one-time secrets setup](../README.md#2-set-backend-authentication-and-local-secrets). The main guide also covers creating the database containers when they do not exist yet.
- **Frontend cannot reach the API:** confirm the backend is running on `http://localhost:8080` and start the frontend with `pnpm dev`.
- **Port already in use:** check whether the application is already running. Do not start another backend/frontend on the same ports.
- **AWS file features:** the recommended local setup disables file sync and supplies dummy SDK settings, so no cloud access is needed for login or the customer screens. S3 synchronization and file downloads require your own AWS configuration.

### Stop and restart

Press **Ctrl+C** in each application terminal. To stop the database and Redis too:

```powershell
docker stop oneview-db oneview-redis
```

Repeat steps 1 and 2 to restart. The saved secrets and database contents persist when containers are stopped and started. Avoid deleting/recreating the database container without a backup.

See the [full workspace guide](../README.md) for backups, image transfer, configuration variables, tests, and optional AWS setup.

---

**OneView Backend** is a layered .NET solution following the principles of **Clean Architecture** and the traditional **Three-Layer Architecture**.  
The solution is designed for scalability, maintainability, and clear separation of concerns between layers.

---

## 📁 Project Structure

```
OneView-Backend.slnx
│
├── BusinessLogic/
├── DataAccess/
├── DTO/
├── OneView-API/
└── Test/
```

### 🔗 Dependency Flow

```
OneView-API
   ↓
BusinessLogic
   ↓
DataAccess
   ↳ DTO (shared contracts)
```

---

## 🧠 BusinessLogic

Contains the **core business rules and logic** of the application.  
This layer is independent of both the database and the API framework.

| Folder | Purpose |
|--------|----------|
| **Exception/** | Custom exception types for business or domain-related errors. |
| **Helper/** | Utility classes and shared logic (e.g., formatting, conversions). |
| **Interface/** | Service interfaces (e.g., `ICustomerService`, `IAgreementService`). |
| **Mapper/** | Logic for mapping between DTOs and internal domain models. |
| **Service/** | Main service implementations that contain business rules and operations. |

---

## 🗃️ DataAccess

Handles **data persistence and retrieval** using Entity Framework Core and other data sources.  
Responsible for connecting the business logic to the database or external data providers.

| Folder | Purpose |
|--------|----------|
| **Context/** | Contains the `DbContext` and database configuration. |
| **Interface/** | Repository interfaces defining data access contracts. |
| **Repository/** | Repository implementations with CRUD and query operations. |
| **Model/** | Entity classes representing database tables. |
| **Mapper/** | Mapping logic between EF entities and DTOs or domain models. |
| **Integration/** | External connectors or API clients (e.g., Freshdesk, Keeper Security). |

---

## 📦 DTO

Defines **Data Transfer Objects** used between layers and as contracts for API communication.  
This project ensures a consistent and isolated data model, independent from database entities.

| Folder | Purpose |
|--------|----------|
| **Customer/** | DTOs for customer management (overview, details, create/update). |
| **File/** | DTOs for file metadata or file handling. |
| **Freshdesk/** | DTOs for Freshdesk integration. |
| **KeeperSecurity/** | DTOs for Keeper Security integration. |
| **Shared/** | Common reusable DTOs (e.g., `AddressDto`, `ContactDto`, `PaginationDto`). |

---

## 🌐 OneView-API

Hosts the **ASP.NET Core Web API** for the OneView platform.  
This layer exposes endpoints, handles requests, and connects the frontend or external clients to business logic.

| Folder | Purpose |
|--------|----------|
| **Configuration/** | Setup classes for Swagger, AutoMapper, CORS, etc. |
| **Controllers/** | ASP.NET Core controllers that define REST endpoints. |
| **Middleware/** | Custom middleware for exception handling, logging, or request tracking. |

**Key files**
- `Program.cs` – Application entry point and startup configuration  
- `appsettings.json` – Environment-specific configuration (connection strings, logging, etc.)  
- `Dockerfile` – Container configuration for deployment  

---

## 🧪 Test

Contains **automated tests** for both unit and BDD (Behavior Driven Development) testing using *Reqnroll*.

| Folder | Purpose |
|--------|----------|
| **Drivers/** | Shared drivers and context used across test steps. |
| **Features/** | `.feature` files written in Gherkin syntax describing scenarios. |
| **StepDefinitions/** | Step implementations connecting feature files to real code. |
| **Support/** | Shared hooks and setup for Reqnroll (test initialization, cleanup). |

---

## 🧩 Naming Conventions

| Type | Convention |
|------|-------------|
| **Folder Names** | Singular (e.g., `Controller`, `Service`, `Repository`) for consistency with .NET namespaces. |
| **DTO Classes** | End with `Dto` (e.g., `CustomerDto`, `AgreementDto`). |
| **Repositories** | End with `Repository` (e.g., `CustomerRepository`). |
| **Services** | End with `Service` (e.g., `CustomerService`). |
| **Extensions** | Pluralized when grouping multiple extension classes (e.g., `Extensions`, `Configurations`). |

---

## 🧱 Architectural Principles

- **Separation of Concerns** – Each layer has a single responsibility.  
- **Dependency Inversion** – High-level modules depend on abstractions, not implementations.  
- **Testability** – Each component can be tested independently.  
- **Scalability** – Easy to extend with new services, modules, or integrations.  
- **Maintainability** – Clean, organized folder structure with clear roles.
