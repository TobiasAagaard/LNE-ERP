using ErpCli.Models;
using TECHCOOL.UI;

namespace ErpCli.Views
{
    public class CompanyDetails : Screen
    {
        public override string Title {get;set;} = "Virksomhedsdetaljer";
        Company company = new();

        public CompanyDetails(Company company)
        {
            Title = "Detaljer for " + company.Name;
            this.company = company;
        }

        protected override void Draw()
        {
            
            Console.WriteLine(company.Name);
            Console.WriteLine("Adresse:");
            Console.WriteLine("{0} {1}", company.Street, company.Number);
            Console.WriteLine("{0} {1}", company.City, company.Country);
            Console.WriteLine("Valuta: {0}", company.Currency);

            Console.WriteLine("Tryk på F2 for at redigere virksomhedens detaljer");
            AddKey(ConsoleKey.F2, () =>
            {
                Screen.Display(new CompanyEditScreen(company));
                Quit();
            });

            ExitOnEscape();


        }
    }
}