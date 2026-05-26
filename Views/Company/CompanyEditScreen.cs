using TECHCOOL.UI;
using ErpCli.Models;
using ErpCli.Data;

namespace ErpCli.Views
{
    public class CompanyEditScreen : Screen
    {
        public override string Title { get; set; } = "Rediger Virksomhed";
        Company company = new();

        public CompanyEditScreen(Company company)
        {
           if (company != null && company.Id != 0)
            {
                Title = "Rediger " + company.Name;
                this.company = company;
            }
            this.company = company ?? new Company();
        }

        protected override void Draw()
        {
            ExitOnEscape();
            Form<Company> form = new();

            form.TextBox("Navn", nameof(Company.Name));
            form.SelectBox("Valuta", nameof(Company.Currency));
            form.AddOption("Valuta", "DKK", Currency.DKK);
            form.AddOption("Valuta", "EUR", Currency.EUR);
            form.AddOption("Valuta", "USD", Currency.USD);
            form.AddOption("Valuta", "SEK", Currency.SEK);
            form.TextBox("Land", nameof(Company.Country));
            form.TextBox("Gade", nameof(Company.Street));
            form.TextBox("Nummer", nameof(Company.Number));
            form.TextBox("Postnummer", nameof(Company.PostalCode));
            form.TextBox("By", nameof(Company.City));
            if (form.Edit(company))
            {
                if (string.IsNullOrEmpty(company.Name)
                    || string.IsNullOrEmpty(company.Street)
                    || string.IsNullOrEmpty(company.Number)
                    || string.IsNullOrEmpty(company.PostalCode)
                    || string.IsNullOrEmpty(company.City)
                    || string.IsNullOrEmpty(company.Country))
                {
                    Console.WriteLine("Navn og adresse må ikke være tomme");
                    return;
                }
                if (company.Id != 0)
                {
                    Database.Instance.UpdateCompany(company);
                }
                else
                {
                    Database.Instance.CreateCompany(company);
                }
                Console.WriteLine("Ændringerne blev gemt");
            }
            else
            {
                Console.WriteLine("Ingen ændringer");
            }
            
            
        }
    }
}