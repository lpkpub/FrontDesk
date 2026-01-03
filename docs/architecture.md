# Architecture

## Projects
- FrontDesk.Api: ASP.NET Core Web API, Controllers, Middleware, Services, Logging
- FrontDesk.Db: PostgreSQL, EF Core Entities (Tables), DbContext, Migrations
- FrontDesk.Shared: DTOs, Enums, Constants, Static helpers
- FrontDesk.Ui: Avalonia Desktop UI

## Dependency
- Api -> Shared, Db
- Db -> Shared
- Shared (none)
- Ui -> Shared