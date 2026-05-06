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
            Screen.Display(new MainMenu());  
        }
    }
}

