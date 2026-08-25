# Agricultural Field Management API

## Project overview

Agricultural Field Management API is an ASP.NET Core REST API for managing agricultural fields, growers, field coordinates, comments, and users. It combines the field-management domain with ASP.NET Identity, JWT authentication, role-based access control, email-based account workflows, and interactive OpenAPI documentation.

This repository contains the backend API only. It does not include a web or mobile frontend.

## Main features

- Field and grower management
- Ordered field coordinates and field comments
- User registration and login
- Email confirmation and password-reset workflows
- JWT bearer authentication
- Role-based authorization
- Database-backed API permission matrix
- Swagger/OpenAPI documentation

## Technology stack

- ASP.NET Core 6 Web API
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- JWT Bearer authentication
- AutoMapper
- MailKit/MimeKit
- Swagger/OpenAPI with Swashbuckle

## Architecture

The solution separates HTTP concerns, application logic, persistence, and email delivery:

- **Controllers** define the authentication, user, and field-management HTTP APIs.
- **Services** contain user, authentication, permission, field, comment, and coordinate operations.
- **DTOs** define request and response shapes for users, fields, comments, and coordinates.
- **Entities** model application users, roles, fields, coordinates, comments, API definitions, and permission mappings.
- **Entity Framework Core** provides SQL Server persistence, Identity storage, relationships, seeded roles and permissions, and schema migrations.
- **AutoMapper** maps between entities and DTOs.
- The separate **Services** project contains the MailKit/MimeKit email service and its configuration model.

The solution file is `BasicStructure/BasicStructure.sln`. The main Web API project is in `BasicStructure/BasicStructure`, and the email-supporting class library is in `BasicStructure/Services`.

## Getting started

### Prerequisites

- .NET 6 SDK
- SQL Server or SQL Server Express
- The `dotnet-ef` CLI tool if you need to apply or manage migrations
- SMTP credentials for an account you control if you want to use email confirmation or password reset

### Database configuration

The API reads its database connection from `ConnectionStrings:DefaultConnection`. The checked-in `appsettings.json` contains a local SQL Server Express configuration. Override it for your environment through an environment variable or another non-committed configuration provider:

```powershell
$env:ConnectionStrings__DefaultConnection = "<your-sql-server-connection-string>"
```

Do not commit production connection strings or database credentials.

### Restore dependencies and apply migrations

From the repository root:

```powershell
dotnet restore BasicStructure/BasicStructure.sln
dotnet ef database update --project BasicStructure/BasicStructure/BasicStructure.csproj
```

The repository includes migrations for Identity roles, the API permission matrix, and field management.

### Run the API

Provide the required JWT settings described in [Configuration](#configuration), then run:

```powershell
dotnet run --project BasicStructure/BasicStructure/BasicStructure.csproj
```

The Development launch profile uses:

- `https://localhost:7051`
- `http://localhost:5051`

Swagger UI is available in the Development environment at `/swagger`, for example `https://localhost:7051/swagger`.

## Configuration

The repository does not contain working credentials. Supply your own JWT signing settings and, if you use registration confirmation or password-reset email, credentials for an SMTP account you control.

ASP.NET Core converts double underscores in environment-variable names to configuration section separators. Set these variables in your shell, IDE launch profile outside source control, deployment platform, or secret manager:

| Environment variable | Configuration key | Required |
|---|---|---|
| `Jwt__Key` | `Jwt:Key` | Yes |
| `Jwt__Issuer` | `Jwt:Issuer` | Yes |
| `Jwt__Audience` | `Jwt:Audience` | Yes |
| `Smtp__Host` | `Smtp:Host` | For email |
| `Smtp__Port` | `Smtp:Port` | For email |
| `Smtp__Username` | `Smtp:Username` | For email |
| `Smtp__Password` | `Smtp:Password` | For email |
| `Smtp__From` | `Smtp:From` | Optional; defaults to username |

### .NET User Secrets

For local development, .NET User Secrets avoids putting values in files:

```powershell
cd BasicStructure/BasicStructure
dotnet user-secrets set "Jwt:Key" "<your-long-random-key>"
dotnet user-secrets set "Jwt:Issuer" "https://localhost:7051"
dotnet user-secrets set "Jwt:Audience" "https://localhost:7051"
dotnet user-secrets set "Smtp:Host" "smtp.gmail.com"
dotnet user-secrets set "Smtp:Port" "465"
dotnet user-secrets set "Smtp:Username" "<your-account>"
dotnet user-secrets set "Smtp:Password" "<your-app-password>"
```

If Gmail is used, create an app password for an account you own and follow Google's current account-security requirements. Never put the value in `appsettings.json`, a committed launch profile, or this README.

The application stops with a clear error if required JWT settings are missing. SMTP settings are validated when email is sent, so non-email operations can run without SMTP; an email attempt with incomplete settings throws a configuration error listing the missing keys.

The placeholder-only `appsettings.example.json` documents the expected configuration shape. `appsettings.Local.json`, `.env`, and `.env.*` are ignored as local secret/configuration files. Ordinary .NET applications do **not** automatically load `.env` files, and this project does not add a dotenv package or loader. Prefer environment variables or .NET User Secrets unless dotenv loading is explicitly implemented later. `.env.example` and `appsettings.example.json` remain eligible for source control when they contain placeholders only.

## Scope

This repository contains the backend REST API for agricultural field management, including authentication, authorization, field operations, comments, coordinates, email workflows, and Swagger/OpenAPI documentation.

The API is designed to serve as a backend foundation for web or mobile clients. Environment-specific values such as database, JWT, and SMTP settings are supplied through environment variables or .NET User Secrets.
