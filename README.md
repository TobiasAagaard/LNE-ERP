# LNE — ERP
The current implementation is a command-line app (`LNE.Cli`) built in C# / .NET 10, covering company, product, customer, and sales-order management.

## Repository structure

```
ERP-CLI/
├── LNE.slnx                 # Solution file
├── README.md                # You are here — project + repo overview
├── .github/workflows/       # CI (build/test)
└── src/
    ├── LNE.Cli/             # Console ERP app (Views / Models / Data + SQL migrations)
    └── LNE.Test/            # xUnit unit tests
```

The layout under `src/` leaves room for future projects (e.g. `LNE.Api`, `LNE.Web`) alongside the existing CLI and test projects.

## Projects

| Project | Type | Description |
|---------|------|-------------|
| [`LNE.Cli`](src/LNE.Cli/) | Console app | The ERP application — company, product, customer, and sales-order management over SQL Server. See its [README](src/LNE.Cli/README.md) for setup and architecture. |
| [`LNE.Test`](src/LNE.Test/) | xUnit tests | Unit tests for domain logic (order totals, profit calculations). |

## Getting started

Full prerequisites, database setup, and run instructions live in the [CLI README](src/LNE.Cli/README.md). In short:

```bash
dotnet build LNE.slnx
dotnet run --project src/LNE.Cli/LNE.Cli.csproj
dotnet test
```

### TECHCOOL dependency

`LNE.Cli` builds on **TECHCOOL**, the console UI library providing the `Screen` and `Menu` primitives. [`LNE.Cli.csproj`](src/LNE.Cli/LNE.Cli.csproj) references it conditionally, so no extra setup is required by default:

- **Sibling folder present** — if a `TECHCOOL/` folder exists next to `ERP-CLI/` (i.e. `..\..\..\TECHCOOL\TECHCOOL.csproj`), it's used as a local `ProjectReference`. Clone it alongside this repo when you want to work against TECHCOOL source:

  ```bash
  git clone https://github.com/TobiasAagaard/TECHCOOL.git
  git clone https://github.com/TobiasAagaard/ERP-CLI ERP-CLI
  ```

  ```
  Projects/
    TECHCOOL/   # local source — used automatically when present
    ERP-CLI/
  ```

- **No sibling folder** — the build falls back to the published `TECHCOOL` NuGet package (`PackageReference` with `Version="*"`), so a plain clone of `ERP-CLI` builds on its own.

## Contributors

[![Contributors](https://contrib.rocks/image?repo=TobiasAagaard/ERP-CLI)](https://github.com/TobiasAagaard/ERP-CLI/graphs/contributors)
</content>
</invoke>
