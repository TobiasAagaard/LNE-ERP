using ErpCli.Models;
using TECHCOOL.UI;

namespace ErpCli.Views;
    public class CustomerDetailsScreen : Screen
    {
        public override string Title { get; set; } = "Kunde Detaljer";

        Person customer = new();
        public CustomerDetailsScreen(Person customer)
        {
            Title = "Detaljer for " + customer.FullName;
            this.customer = customer;
        }

        protected override void Draw()
        {
            Console.WriteLine(customer.FullName);
            Console.WriteLine("Email: {0}", customer.Email);
            Console.WriteLine("Telefon: {0}", customer.Phone);
            Console.WriteLine("Virksomhed: {0}", customer.Company?.Name ?? "Ingen virksomhed tilknyttet");
            Console.WriteLine("Adresse:");
            Console.WriteLine("{0} {1}", customer.Street, customer.Number);
            Console.WriteLine("{0} {1}", customer.City, customer.Country);

            Console.WriteLine();
            Console.WriteLine("Tryk på F2 for at redigere kundens detaljer");
            AddKey(ConsoleKey.F2, () =>
            {
                Screen.Display(new CustomerEditScreen(customer));
                Quit(); 
            });

            ExitOnEscape();
        }
    }


