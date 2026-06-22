# Ambs Reports

## Tech stack

.NET 8 Clean Architecture. Legacy .NET Framework 4.6.1 library (`AmbsLibrary/`).

- **Data**: EF Core 9 (SQL Server) for writes, Dapper (`ApplicationReadDbContext`) for reads — both against SQL Server.
- **Background jobs**: Hangfire on PostgreSQL (`ConnectionStrings:HangfireConnection`). Queue config in `HangfireConfigs[]` (queues: high, low, default).
- **CQRS**: MediatR commands/queries + `ICacheService` deduplication via `JobDeduplicationService`.
- **Storage**: Minio (S3-compatible) — files stored in `reports` bucket.
- **Notifications**: SignalR hub at `/hubs/notifications`, plus webhooks to external AMBS system (`AmbsConfig`).
- **Logging**: Serilog, OS-aware sections `SerilogWindows` / `SerilogLinux`.
- **Excel**: ClosedXML + EPPlus + legacy `Asa.ExcelXmlWriter` (DLL in `ExternalDll/`).
- **Testing**: xUnit + coverlet (3 stub projects, no real tests yet).

## Build & test

Run from `AmbsReports/`:

```
dotnet build AmbsReports.sln
dotnet test AmbsReports.sln
```

SDK-style projects inherit `net8.0` from `Directory.Build.props`. The legacy `AmbsLibrary.csproj` (net461, non-SDK-style) is unaffected.

## Path quirks

- `src/Infrastruture/` — **not** `Infrastructure/` (typo in directory name).
- `AmbsLibrary.csproj` references DLL from `src/Infrastruture/AmbsReports.Infrastructure/ExternalDll/`.

## Config quirks

- **Connection strings are encrypted** — encrypted `Password=` value in `appsettings.json`; decrypted at runtime by `CryptoUtils.DecryptConnectionString()` (TripleDES, hardcoded key/IV).
- **Dual SQL connections** — `SqlConnection` (main DB) and `ReportingSqlConnection` (reporting DB); which one is used depends on `CreateReportCommand.ExecuteFromReportingDb`.
- **Commit editable appsettings with dummy/encrypted values** — real secrets are environment-specific.
- **CORS** is locked to `http://localhost:3000` and `http://localhost:16509` (credentialed).

## Code conventions (from `.editorconfig`)

- `_camelCase` private/internal fields, `s_` prefix for static fields.
- `var` **discouraged** even when type is apparent (`false:none`).
- No `this.` qualification.
- Expression-bodied members preferred.
- Primary constructors preferred.
- License header required: `Licensed to the .NET Foundation under one or more agreements.\nThe .NET Foundation licenses this file to you under the MIT license.`

## Architecture flow

1. `ReportController.Create()` receives `CreateReportCommand` via POST.
2. `JobDeduplicationService.TryEnqueueUniqueJob()` checks cache, then enqueues a Hangfire job via `IMediator.Enqueue()` on the specified queue.
3. `CreateReportCommandHandler` runs inside Hangfire: generates Excel via `ExcelUtility`, uploads to Minio via `IStorageService`, saves backup to DB, stores `P_Notices`, and calls `WebhookService.PublishToWebhookAsync()`.
4. Client polls `GET /api/report?Token=&JobId=&ReportName=` → `GetReportQueryHandler` checks Hangfire status and returns a presigned URL.

## Known inconsistencies

- Namespace mix: `AMBS.Reports.Repositories.Interfaces` coexists with `AmbsReports.*`.
- Block-scoped and file-scoped namespaces both used (file-scoped preferred per .editorconfig but not enforced).
- `NotifyExternalSystemService` and `CryptoUtils` have both instance and static methods.
- Test csprojs reference no project references — only NuGet packages. They will not compile against real code.
