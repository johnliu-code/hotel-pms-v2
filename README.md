# Hotel PMS 2.0

Hotel PMS 2.0 is a modern, API-first Property Management System designed initially for small and mid-sized hotels, motels, inns, and short-term rental operators. Built with .NET, ASP.NET Core, .NET MAUI, and React, the project focuses on reservation management, room availability, guest operations, housekeeping, billing, multi-channel booking management, and secure remote access. Its modular and integration-ready architecture is designed to support future growth, including external booking platforms, channel managers, and larger hotel operations.

## Goals and target users

Build an API-first property management system for small and mid-sized hotels, motels, inns, and short-term rental operators. Future capabilities include reservations, availability, guest operations, housekeeping, billing, and multi-channel booking. HPMS-17 established the buildable foundation; HPMS-20 adds shared Domain primitives without implementing business entities or workflows.

## Architecture and technology

- API-first modular monolith in C# on .NET 10 LTS and ASP.NET Core.
- Domain is independent; Application depends on Domain; Infrastructure implements technical concerns; API is the composition root.
- EF Core 10 is referenced only by Infrastructure. A relational SQL provider will be selected later; no database or credentials are currently required.
- .NET MAUI is the primary operational client; React and TypeScript with Vite provide a complementary web/admin client.
- Both clients communicate through HTTP/JSON APIs only. They have no backend project references and must never access the database directly.
- ASP.NET Core OpenAPI describes the API in Development.

See [architecture](docs/architecture/README.md), [database direction](docs/database/README.md), and [API notes](docs/api/README.md).

## Solution structure

```text
hotel-pms-v2/
├── docs/
│   ├── requirements/
│   ├── architecture/
│   ├── database/
│   ├── api/
│   ├── development/
│   └── security/
├── src/
│   ├── HotelPms.Domain/
│   ├── HotelPms.Application/
│   ├── HotelPms.Infrastructure/
│   ├── HotelPms.Api/
│   ├── HotelPms.Maui/
│   └── HotelPms.Web/
├── tests/
│   ├── HotelPms.UnitTests/
│   └── HotelPms.IntegrationTests/
├── HotelPms.sln
├── global.json
├── README.md
└── .gitignore
```

`HotelPms.sln` contains the four backend projects and two test projects. MAUI and Web are built independently so backend development does not require native client workloads. Domain contains reservation lifecycle and room operational status types plus immutable Money/Currency values; see the [domain decisions](docs/architecture/DOMAIN_DECISIONS.md). Application/Infrastructure contain no placeholder business classes. UnitTests exercises Money/Currency invariants; IntegrationTests contains two API health/OpenAPI tests. Persistence and infrastructure tests will accompany actual implementations.

## Prerequisites

- .NET 10 SDK: `global.json` requires stable 10.0.100 or a later .NET 10 feature band. This skeleton was validated with SDK 10.0.401.
- Node.js 24 LTS and npm for the web client; commit and use the npm lockfile.
- Git and access to NuGet/npm registries for dependency restore.
- For MAUI: .NET 10 MAUI workload plus target-specific tools. Windows requires a supported Windows machine and Windows SDK; iOS/Mac Catalyst require macOS and a compatible Xcode; Android requires the Android SDK and supported JDK. Consult the [official MAUI installation guide](https://learn.microsoft.com/dotnet/maui/get-started/installation).

## Backend build, test, and run

From the repository root:

```sh
dotnet restore HotelPms.sln
dotnet build HotelPms.sln --configuration Release --no-restore
dotnet test HotelPms.sln --configuration Release --no-build
# Explicit local HTTP profile for development only:
dotnet run --project src/HotelPms.Api --launch-profile http
```

The HTTP launch profile uses `http://localhost:5000`. Verify:

```sh
curl http://localhost:5000/health
curl http://localhost:5000/openapi/v1.json
```

The HTTP-only profile can log an HTTPS-redirection warning because no HTTPS port is configured; use the HTTPS profile when testing TLS locally.

The health response is `{"status":"Healthy"}`. It indicates process availability only. OpenAPI is available only in Development. For local HTTPS, run `dotnet dev-certs https --trust` on a supported workstation and use the `https` launch profile. Production hosting must configure HTTPS; no authentication or business routes are present yet.

## Web build and run

```sh
cd src/HotelPms.Web
npm ci
npm run build
npm run lint
npm run dev
```

The client displays only a landing screen. API calls and browser authentication/CORS configuration are deferred until real use cases exist; no database connection or client secrets belong here.

## MAUI setup and build

On a supported development workstation, install the .NET 10 MAUI workload and native prerequisites:

```sh
dotnet workload install maui
# Windows target, on Windows:
dotnet build src/HotelPms.Maui/HotelPms.Maui.csproj -f net10.0-windows10.0.19041.0
# Android target, with Android SDK/JDK configured:
dotnet build src/HotelPms.Maui/HotelPms.Maui.csproj -f net10.0-android
# iOS simulator target, on macOS with compatible Xcode:
dotnet build src/HotelPms.Maui/HotelPms.Maui.csproj -f net10.0-ios -p:RuntimeIdentifier=iossimulator-arm64
```

On Linux, Android development requires the `maui-android` workload rather than the full `maui` workload. Windows and Apple targets cannot be built on Linux. The cloud environment has no MAUI workload or native SDKs; MAUI compilation and execution have **not** been validated. The project uses the official .NET 10 MAUI template and a minimal static screen, with no business behavior or backend project references. Use an IDE or emulator/device configured for the target to run it. The default application identifier is a development placeholder; choose a production identifier before distribution.

## Legacy prototype and future coexistence

This repository is the new Hotel PMS 2.0 foundation. The legacy prototype remains a separate repository and is not modified or referenced by this solution. Migration, shared data ownership, and coexistence contracts are future decisions; no direct shared-database access is assumed.

Future integrations with booking platforms, channel managers, and external PMS providers should use application contracts and infrastructure adapters. Translate provider-specific payloads at those boundaries and keep vendor dependencies out of Domain. HTTP APIs can support gradual coexistence and migration once contracts, authentication, and ownership rules are agreed.

## Source control and secrets

HPMS-17 work belongs on `feature/HPMS-17-solution-skeleton` and is reviewed before merging. Do not commit secrets, certificates, signing material, local environment files, or build outputs. Future server credentials should use user secrets or deployment secret storage; never embed credentials in either client.

See the [security and payment-data boundaries](docs/security/SECURITY_AND_PAYMENT_BOUNDARIES.md) for the backend security baseline, protected data, payment handling, and integration requirements.

## Development standards

Follow the [development standards and Definition of Done](docs/development/DEVELOPMENT_STANDARDS.md) for source control, architecture, testing, review, AI-assisted development, and documentation requirements.
