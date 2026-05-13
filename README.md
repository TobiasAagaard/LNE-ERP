# ERP-CLI — LNE Security
 
A command-line ERP (Enterprise Resource Planning) system built in C# / .NET 10 for the fictional company **LNE Security A/S**, a Danish IT-services company headquartered in Aalborg.
 
The application is designed as a foundation that can be extended to other industries, so the architecture is kept clean and modular to support multiple developers working in parallel and several future versions of the product.

## Getting Started

### Prerequisites

- [.NET 10 SDK (preview)](https://dotnet.microsoft.com/download/dotnet/10.0)
- A local clone of our fork of [TECHCOOL](https://github.com/TobiasAagaard/TECHCOOL). The project references the fork as a sibling folder rather than the TECHCOOL NuGet package, so cloning is required for the full experience and all features to work.

### Build and run

1. Create a parent folder to hold both repositories side by side:

   ```
   Projects/
     TECHCOOL/
     ERP-CLI/
   ```

2. Clone both repositories into that folder:

   ```bash
   git clone https://github.com/TobiasAagaard/TECHCOOL.git
   git clone https://github.com/TobiasAagaard/ERP-CLI ERP-CLI
   ```

3. Create an `appsettings.Local.json` file in the root of `ERP-CLI/` with your database connection details:

   ```json
   {
     "Database": {
       "DataSource": "<server>",
       "UserId": "<user>",
       "Password": "<password>",
       "InitialCatalog": "<database>"
     }
   }
   ```

   This file is git-ignored so each developer can keep their own local credentials.

4. From inside `ERP-CLI/`, build and run:

   ```bash
   dotnet build
   dotnet run
   ```

## Application flow

The application is structured in three layers — **Views**, **Models**, and **Data** — wired together by a singleton `Database.Instance` that talks to SQL Server. Navigation happens through TECHCOOL's `Screen` and `Menu` UI primitives.

### Navigation

```
Program.cs
   └── MainMenu
         ├── CompanyListScreen   ──► CompanyDetailsScreen   ──► CompanyEditScreen
         ├── ProductListPage     ──► ProductDetailsScreen   ──► ProductEditorScreen
         ├── SalesListScreen     ──► SalesDetailsScreen     ──► SalesEditScreen
         │                                 └── OrderLineEditScreen
         └── CustomerListScreen  ──► CustomerDetailsScreen  ──► CustomerEditScreen
```

Every `ListScreen` follows the same pattern:

1. Loads its records via `Database.Instance.GetAll…()` and renders them in a `ListPage<T>`.
2. Registers function keys for actions — typically **F1/F3** to create, **F2** to edit, **F5** to delete.
3. Selecting a row (`Enter`) opens the corresponding `DetailsScreen`; **Esc** returns to the previous screen.

### Sales order specifics

A sales order is a header (`SalesOrderHeader`) with many lines (`OrderLine`):

1. From `SalesListScreen` press **F3** to create a header — pick a customer (search box) and a status. Saving inserts the header.
2. Open the header to reach `SalesDetailsScreen`, which lists existing lines and exposes **F3** (edit line), **F4** (new line), **F5** (delete line).
3. Each line is edited in `OrderLineEditScreen`, which writes through `Database.Instance` to the `SalesOrderLines` table.
4. Setting the header status to `Færdig` stamps `OrderCompletedAt` automatically.

### Configuration

Database credentials are loaded once at type initialization from `appsettings.Local.json` (see step 3 of *Build and run*). The file is git-ignored, so each developer keeps their own copy.

## Tests

Unit tests live in the [ERP-CLI.Tests](ERP-CLI.Tests/) project and use [xUnit](https://xunit.net/).

### Run the tests

```bash
dotnet test
```

### Naming convention

We follow the `MethodName_Scenario_ExpectedBehavior` naming convention

- **MethodName** — the method or property under test
- **Scenario** — the input or state being exercised 
- **ExpectedBehavior** — the observable result

Each test should follow the **Arrange–Act–Assert** structure so the three phases are easy to read.


## Contributors
 
- [Nicklas](https://github.com/NickRaics)
- [Tobias](https://github.com/TobiasAagaard)
- [Malthe](https://github.com/Malthebk3)
