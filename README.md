# ERP-CLI — LNE Security

A command-line ERP system built in C# / .NET 10 for **LNE Security A/S**, a fictional Danish IT-services company based in Aalborg. The architecture is modular, designed to support multiple developers.

## Prerequisites

- [.NET 10 SDK (preview)](https://dotnet.microsoft.com/download/dotnet/10.0)
- Microsoft SQL Server (mixed-mode authentication enabled)
- A local clone of the [TECHCOOL fork](https://github.com/TobiasAagaard/TECHCOOL) — the project references it as a sibling folder, not a NuGet package

## Setup

**1. Clone both repos side by side**

```
Projects/
  TECHCOOL/
  ERP-CLI/
```

```bash
git clone https://github.com/TobiasAagaard/TECHCOOL.git
git clone https://github.com/TobiasAagaard/ERP-CLI ERP-CLI
```

**2. Start SQL Server and run the schema**

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<Your@Password123>" \
  -p 1433:1433 --name erp-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

Then run [`Sql/FullScript.sql`](Sql/FullScript.sql) to create the `ERP_CLI` database and all tables.

**3. Create `appsettings.Local.json`** in the root of `ERP-CLI/` (git-ignored):

```json
{
  "Database": {
    "DataSource": "localhost",
    "UserId": "sa",
    "Password": "<YourPassword>",
    "InitialCatalog": "ERP_CLI"
  }
}
```

**4. Build and run**

```bash
dotnet build
dotnet run
```

## Architecture

Three layers — **Views**, **Models**, **Data** — wired through a `Database.Instance` singleton. Navigation uses TECHCOOL's `Screen` and `Menu` primitives.

```
MainMenu
  ├── CompanyListScreen   → CompanyDetailsScreen   → CompanyEditScreen
  ├── ProductListPage     → ProductDetailsScreen   → ProductEditorScreen
  ├── SalesListScreen     → SalesDetailsScreen     → SalesEditScreen
  │                                └── OrderLineEditScreen
  └── CustomerListScreen  → CustomerDetailsScreen  → CustomerEditScreen
```

Each list screen loads records, registers function keys (F1/F3 create, F2 edit, F5 delete), and opens a details screen on `Enter`.

Sales orders are a header (`SalesOrderHeader`) with many lines (`OrderLine`). Setting status to `Færdig` stamps `OrderCompletedAt` automatically.

## Tests

Unit tests use [xUnit](https://xunit.net/) and follow `MethodName_Scenario_ExpectedBehavior` naming with Arrange–Act–Assert structure.

```bash
dotnet test
```

## Contributors

- [Nicklas](https://github.com/NickRaics)
- [Tobias](https://github.com/TobiasAagaard)
- [Malthe](https://github.com/Malthebk3)
