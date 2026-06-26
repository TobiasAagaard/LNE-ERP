# LNE — ERP for LNE Security A/S
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

## Contributors

[![Contributors](https://contrib.rocks/image?repo=TobiasAagaard/ERP-CLI)](https://github.com/TobiasAagaard/ERP-CLI/graphs/contributors)
</content>
</invoke>
