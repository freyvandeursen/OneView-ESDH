# OneView - ESDH

*ESDH Prototype · Customer & Document Management*

OneView brings customer records, contact information, labels, and related documents together in a unified workspace. It is an ESDH-inspired prototype: **ESDH** stands for *Elektronisk Sags- og Dokumenthåndtering* (Electronic Case and Document Management).

**OneView is a group project developed by a team of six people in collaboration with Vitani.** This repository presents the shared project and documents how to run it locally. The work is credited to the group as a whole.

OneView is a React customer-management frontend backed by an ASP.NET Core API, SQL Server, and Redis. The backend uses Entity Framework Core for database access. AWS S3 provides an optional file integration.

## Project overview

| Area | Implemented functionality |
| --- | --- |
| Customer management | Customer overview, details, creation, activation, and deactivation |
| Contacts | Customer-linked contact information |
| Organization | Labels and contextual information attached to customers and documents |
| Document access | S3-backed document discovery, metadata, and signed download links |
| Authentication | JWT-based login with user and administrator roles |

The current implementation focuses on customer management and document access. Structured case processing is not yet implemented. The ESDH label describes the prototype's direction and context.

## Local service addresses

The setup below starts these services. Local verification on 27 October 2026 confirmed successful browser login, customer retrieval, and database connectivity.

| Service | Address | Verification |
| --- | --- | --- |
| React frontend | http://127.0.0.1:5173/ | Login and customer list |
| ASP.NET Core API | http://localhost:8080/ | Login and authenticated customer requests |
| Swagger | http://localhost:8080/swagger | API documentation |
| API/database health | http://localhost:8080/Health | `Healthy` / `Connected` |
| SQL Server | `localhost,1433`, database `OneView` | Schema version 23 |
| Redis | `localhost:6379` | `PONG` |

Local demo logins:

| Username | Password | Role |
| --- | --- | --- |
| `admin` | `adminpass` | `ADMIN` |
| `user` | `pass` | `USER` |

These are sample accounts seeded by the SQL script, not external authentication-provider accounts. The existing login implementation stores passwords in plain text; use this setup for local development only.

The local setup below disables AWS file synchronization. Core customer, contact, context, label, and login features use the local backend/database. Synchronizing S3 files and opening their download links require your own working S3 configuration; those external operations have not been verified.

## Which folders to use

Run commands from this workspace root unless a step says otherwise:

```text
repository-root/
  README.md                       This guide
  oneview-backend-main/
    OneView-API/                   ASP.NET Core host, controllers, settings
    BusinessLogic/                Services, authentication, background file sync
    DataAccess/                   EF Core models, repositories, SQL updates
    DTO/                          Shared contracts
    Test/                         Service/BDD and JWT tests
    Test_Unit/                    SQL-backed repository tests
    docker-compose.yml            SQL Server and Redis only
  oneview-frontend-main/
    src/                          React application
    vite.config.ts                Development API proxy
  .local/                         Ignored logs, process IDs, backups, verification artifacts
```

The public repository contains the canonical backend and frontend source folders. Duplicate archive snapshots, local backups, credentials, and generated files are excluded. The frontend's old Compose file starts previously published backend infrastructure images; it does not start the frontend. Use the backend Compose file and source commands below.

## Prerequisites

- Docker Desktop running **Linux containers**, with Docker Compose v2.
- .NET 9 SDK/runtime, or a newer SDK with the .NET 9 ASP.NET Core runtime installed. The projects target `net9.0`; this setup was built using SDK 10.0.400 with runtime 9.0.10 installed.
- Node.js 22.12+ or Node.js 24, and pnpm 10. Verified with Node 24.11.0 / pnpm 10.22.0.
- PowerShell for the commands in this guide. PowerShell 5.1 is sufficient.
- Ports 1433, 6379, 8080, and 5173 available.

Check tools with `dotnet --info`, `docker version`, `docker compose version`, `node --version`, and `pnpm --version`. If pnpm is absent, install it with `npm install --global pnpm@10`.

## 1. Start SQL Server and Redis

On a fresh checkout, create the database and Redis containers:

```powershell
docker compose -f .\oneview-backend-main\docker-compose.yml up -d
```

If you already have these project containers, start them without replacing them:

```powershell
docker start oneview-db oneview-redis
docker ps
docker exec oneview-redis redis-cli PING
```

The database image is `ghcr.io/nilshilmarn/oneview-db:latest`; Redis uses `redis:latest`. Wait for SQL Server to finish starting before starting the API. This image contains the `OneView` database and `OneViewApp` SQL login. Supply its connection string privately in the secrets setup below; database passwords are not included in this public repository.

If GHCR refuses a pull, obtain access to that package from its owner or transfer an existing image as described below. Existing local images can be reused without pulling them again.

**Preserve the existing database container.** The supplied Compose configuration has no persistent SQL volume; current data lives in the container's writable layer. `docker stop`/`docker start` preserves it. Removing or recreating that container can lose changes. Make a SQL backup before replacement; adding a volume later does not automatically move its current data.

## 2. Set backend authentication and local secrets

The API now reads the JWT signing key from `Jwt:Key`; both token creation and validation use that same setting. No signing key is compiled into the running source. It must contain at least 32 UTF-8 bytes. Issuer and audience default to `OneViewAPI` and `OneViewClient`.

Run this once from the repository root to save a random JWT key and offline AWS settings in .NET User Secrets for `OneView-API-local-development`. Skip it on routine restarts if you have already configured your own local secrets:

```powershell
$apiProject = '.\oneview-backend-main\OneView-API\OneView-API.csproj'
$secretBytes = New-Object byte[] 48
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
try { $rng.GetBytes($secretBytes) } finally { $rng.Dispose() }
$jwtSecret = [Convert]::ToBase64String($secretBytes)
dotnet user-secrets set 'Jwt:Key' "$jwtSecret" --project $apiProject

# Run without AWS access. The SDK is constructed at startup, so dummy keys are required.
dotnet user-secrets set 'FileSync:Enabled' 'false' --project $apiProject
dotnet user-secrets set 'AWS:AccessKey' 'local-development-unused' --project $apiProject
dotnet user-secrets set 'AWS:SecretKey' 'local-development-unused' --project $apiProject
dotnet user-secrets set 'AWS:ServiceURL' 'http://127.0.0.1:9000' --project $apiProject
dotnet user-secrets set 'AWS:BucketName' 'oneview-local' --project $apiProject
```

Port 9000 above is an unused placeholder, not another service you need to start. Disabling file sync prevents the scheduled AWS calls; it does not emulate S3 downloads.

User Secrets load automatically in the `Development` environment and stay outside the repository. They are a local development store, not encrypted production storage. See [Microsoft's User Secrets documentation](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-9.0).

Configure the database connection using the password for your SQL instance or seeded image. Obtain that password privately from the project/image owner; it is separate from the demo application passwords above:

```powershell
$dbPassword = Read-Host 'SQL password for OneViewApp'
dotnet user-secrets set 'ConnectionStrings:DefaultConnection' "Server=localhost,1433;Database=OneView;User Id=OneViewApp;Password=$dbPassword;Encrypt=True;TrustServerCertificate=True;" --project $apiProject
dotnet user-secrets set 'REDIS:ConnectionString' 'localhost:6379' --project $apiProject
```

The database `OneView` and SQL login must exist. The API creates/updates tables inside that database; it does not create the database or SQL login. The supplied image already contains both. The app login (`admin` / `adminpass`) is separate from the SQL login (`OneViewApp`).

Environment variables also work, including in containers and non-Development environments:

| User Secrets/configuration key | Environment variable | Purpose |
| --- | --- | --- |
| `Jwt:Key` | `Jwt__Key` | Required private signing key |
| `Jwt:Issuer` | `Jwt__Issuer` | Optional issuer override |
| `Jwt:Audience` | `Jwt__Audience` | Optional audience override |
| `ConnectionStrings:DefaultConnection` | `ConnectionStrings__DefaultConnection` | SQL connection string |
| `REDIS:ConnectionString` | `REDIS__ConnectionString` | Redis host/port |
| `FileSync:Enabled` | `FileSync__Enabled` | Defaults to `false` in the supplied settings; enable only with your own S3 configuration |
| `AWS:AccessKey` / `AWS:SecretKey` | `AWS__AccessKey` / `AWS__SecretKey` | S3 credentials |
| `AWS:ServiceURL` / `AWS:BucketName` | `AWS__ServiceURL` / `AWS__BucketName` | S3 endpoint and bucket |

Environment variables override User Secrets and JSON configuration. A backend `.env` file is not automatically loaded by `dotnet run`; use the configuration keys listed above.

For authentication there is no OAuth client ID, client secret, callback URL, or frontend authentication secret to configure. The React app posts credentials to `/api/Login`, keeps the JWT in `sessionStorage`, and sends a Bearer header. Never put the JWT signing key or AWS credentials into a `VITE_*` variable: those variables are exposed to the browser.

Changing the signing key invalidates existing tokens; log in again afterward.

## 3. Start the backend

In terminal 1, from the workspace root:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project .\oneview-backend-main\OneView-API\OneView-API.csproj --no-launch-profile --urls http://localhost:8080
```

This restores/builds the source and listens on port 8080, which matches the existing Vite proxy. The original launch profiles use other ports, so keep `--no-launch-profile --urls ...` here. Local development uses HTTP; no HTTPS certificate setup is needed. A message about not finding an HTTPS redirect port does not prevent this HTTP setup from working.

At startup the API applies pending SQL blocks from `DataAccess/Context/change.sql` and records each successful block in `Versions`. Current schema version: **23**. Each block has its own transaction, so a later failure leaves earlier successful blocks recorded and startup resumes from there.

**Do not run `dotnet ef database update` or add an EF migration for this setup.** The repository has no EF migrations; its custom SQL updater owns schema changes. The old image initially reported version 19 but lacked the `Users` table. Block 23 now repairs that specific mismatch without resetting customer data.

## 4. Start the frontend

In terminal 2, from the workspace root:

```powershell
Set-Location .\oneview-frontend-main
pnpm install --frozen-lockfile
pnpm dev --host 127.0.0.1 --port 5173 --strictPort
```

Open http://127.0.0.1:5173/login and sign in with a demo account. The development environment already sets `VITE_API_URL=/api`. Vite forwards `/api/*` to `http://localhost:8080/*` and removes the `/api` prefix. This same-origin proxy is why local browser requests work with the backend's existing CORS policy.

Use `pnpm build` to check the TypeScript and production bundle. A deployed production build needs its own `VITE_API_URL` and reverse-proxy configuration. `pnpm preview` is not the local setup described here; use the development server for the verified proxy behavior.

## 5. Verify the stack

From the workspace root in another terminal:

```powershell
Invoke-RestMethod http://localhost:8080/Health
Invoke-RestMethod http://127.0.0.1:5173/api/Health
docker exec oneview-redis redis-cli PING

$login = Invoke-RestMethod -Uri http://127.0.0.1:5173/api/Login -Method Post -ContentType 'application/json' -Body '{"username":"admin","password":"adminpass"}'
$customers = Invoke-RestMethod -Uri http://127.0.0.1:5173/api/Customer -Headers @{ Authorization = "Bearer $($login.token)" }
@($customers).Count
```

Expected: database `Connected`, Redis `PONG`, successful login, and three existing customers. An unauthenticated `GET /Customer` returns HTTP 401.

Build and tests:

```powershell
dotnet build .\oneview-backend-main\OneView-API\OneView-API.csproj
dotnet test .\oneview-backend-main\Test\Test.csproj

# Integration tests rebuild the disposable OneViewTest database.
$testDbPassword = Read-Host 'SQL password for OneViewApp on the disposable test database'
$env:ONEVIEW_TEST_CONNECTION_STRING = "Server=localhost,1433;Database=OneViewTest;User Id=OneViewApp;Password=$testDbPassword;Encrypt=True;TrustServerCertificate=True;"
dotnet test .\oneview-backend-main\Test_Unit\Test_Unit.csproj
Remove-Item Env:ONEVIEW_TEST_CONNECTION_STRING

pnpm --dir .\oneview-frontend-main build
```

Verified: backend build with zero warnings/errors; frontend install and production build; 22 `Test` tests (including six JWT checks) and 13 `Test_Unit` tests passing. Browser verification submitted the real login form and displayed all three customers without browser or network errors. Vite's large-bundle warning does not prevent the build.

`Test` uses EF in-memory storage and needs no external services. `Test_Unit` requires the separate, disposable `OneViewTest` database to exist and its SQL login to have DDL access. Each test drops and rebuilds its database objects. Supply its connection string through `ONEVIEW_TEST_CONNECTION_STRING`; the test setup rejects other database names. API User Secrets do not configure this test connection. Keep these tests away from a database containing work you need to preserve.

## Stopping and restarting

Press Ctrl+C in each frontend/backend terminal.

Optionally stop the infrastructure with `docker stop oneview-db oneview-redis`. Restart using steps 1, 3, and 4; secrets persist. Avoid starting a second copy while the existing services own ports 8080/5173.

## Backups and moving to another machine

You do not migrate a Docker image to apply schema changes: the API applies its SQL update script at startup. To move your current database to another machine, create and transfer a SQL backup. Local backups are excluded from this repository.

From the repository root, back up the running database:

```powershell
New-Item -ItemType Directory -Path .local -Force | Out-Null
$backupName = 'OneView-' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '.bak'
$env:SQLCMDPASSWORD = Read-Host 'SQL password for OneViewApp (from your local configuration)'
try {
    docker exec -e SQLCMDPASSWORD oneview-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U OneViewApp -d OneView -C -b -Q "BACKUP DATABASE [OneView] TO DISK = N'/var/opt/mssql/data/$backupName' WITH COPY_ONLY, CHECKSUM;"
    if ($LASTEXITCODE -ne 0) { throw 'Database backup failed.' }
    docker cp "oneview-db:/var/opt/mssql/data/$backupName" ".local/$backupName"
} finally {
    Remove-Item Env:SQLCMDPASSWORD -ErrorAction SilentlyContinue
}
```

The seed image may be pulled on another machine. If package access is unavailable, export/import the image:

```powershell
# Source machine
docker save -o .\oneview-db-image.tar ghcr.io/nilshilmarn/oneview-db:latest
# Destination machine, after copying the tar file there
docker load -i .\oneview-db-image.tar
```

Image export contains the image's original seed data, **not subsequent changes in the running container**. Move a fresh `.bak` as well to retain your current database, then restore it into SQL Server using an administrative SQL login. See [Microsoft's Linux-container backup/restore procedure](https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-migrate-restore-database?view=sql-server-ver17). Keep database backups private; they contain application data and demo login passwords. A future persistent-volume setup should restore this backup into its SQL data volume before replacing the existing container.

## Optional AWS file integration

Runtime secrets and business documents are intentionally excluded from this public repository. Supply your own development credentials through User Secrets or environment variables. No AWS credentials were validated or used for S3 operations during local verification.

To enable your own integration, replace the dummy `AWS:AccessKey`, `AWS:SecretKey`, `AWS:ServiceURL`, and `AWS:BucketName` User Secrets with valid values, then set `FileSync:Enabled` to `true` and restart the API. The endpoint in the original configuration was S3 in `eu-north-1`. The file job lists objects under `Vitani Security/Kunder/`, maps them to customer IDs, and repeats every 15 minutes. Its credentials need bucket-list and object-read permission for the relevant files. Redis caches signed download URLs.

## Troubleshooting and changes made

| Symptom | What to check |
| --- | --- |
| API exits because `Jwt:Key` is missing/short | Run secrets setup for the API project and set `ASPNETCORE_ENVIRONMENT=Development`; use at least 32 UTF-8 bytes. |
| SQL connection/startup failure | Start Docker Desktop/`oneview-db`, wait for SQL readiness, check port 1433 and `ConnectionStrings:DefaultConnection`. |
| `Invalid object name 'Users'` from an older copy | Use the updated source and rebuild. Updated SQL block 23 handles the supplied image's missing table. |
| Frontend proxy `ECONNREFUSED` | The API must listen on `http://localhost:8080`, not just the original launch-profile ports. |
| Browser CORS/old-domain errors | Use the Vite dev server and `/api`; remove conflicting `VITE_API_URL` shell or `.env.*.local` overrides. |
| `401` after key changes | Sign out and sign in again. |
| S3 download/sync errors | File integration needs your own S3 account/configuration; dummy local settings do not serve files. |
| Port already in use | Check existing OneView processes/containers before starting another copy. |

Local-run support includes configurable shared JWT settings and tests, API User Secrets support, a switch for the AWS background job, the old-image user-table repair in SQL block 23, and `AWSSDK.Core` 4.0.3 to 4.0.3.3. That dependency patch fixes the [AWS advisory](https://github.com/aws/aws-sdk-net/security/advisories/GHSA-9cvc-h2w8-phrp) that otherwise makes restore fail under the repository's warnings-as-errors policy. The public snapshot omits duplicate archive copies, business documents, local backups, generated output, and runtime secrets.
