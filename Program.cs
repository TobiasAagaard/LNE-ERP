using TECHCOOL.UI;
using ErpCli.Views;
using ErpCli.Migrations;
using Microsoft.Extensions.Configuration;


namespace ErpCli
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Local.json", optional: false, reloadOnChange: false)
                .Build();

                Migrator.Migrate(config);

            Screen.Display(new MainMenu());  
        }
    }
}

