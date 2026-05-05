using TECHCOOL.UI;
using ErpCli.Views;
using System.Data.SqlClient;
using System.Data.Entity;

namespace ErpCli
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Screen.Display(new MainMenu());
            }
            catch (Exception )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Kunne ikke oprette forbindelse til databasen. Tjek dine databaseindstillinger i appsettings.Local.json og prøv igen.");
                Console.WriteLine("Tryk på en vilkårlig tast for at afslutte.");
                Console.ResetColor();
                Console.ReadKey();
            }
        }
    }
}

