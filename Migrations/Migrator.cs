using DbUp;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace ErpCli.Migrations;

public static class Migrator
{
    public static void Migrate(IConfiguration config)
    {
        IConfigurationSection db = config.GetSection("Database");

        string connectionString =
            $"Server={db["DataSource"]};" +
            $"Database={db["InitialCatalog"]};" +
            $"User Id={db["UserId"]};" +
            $"Password={db["Password"]};" +
            $"TrustServerCertificate=True;";

        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(),
                name => name.Contains("Migrations"))
            .WithTransactionPerScript()
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Migration fejlede: {result.Error}");
            Console.ResetColor();
            Environment.Exit(1);
        }
    }
}