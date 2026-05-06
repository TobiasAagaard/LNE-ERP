using ErpCli.Models;
using TECHCOOL.UI;

namespace ErpCli.Views
{
    public class CustomerDetailsScreen : Screen
    {
        public override string Title { get; set; } = "Kundedetaljer";
        Customer customer = new();

        public CustomerDetailsScreen(Customer customer)
        {
            Title = "Detaljer for " + customer.FullName;
            this.customer = customer;
        }

        protected override void Draw()
        {

            Console.WriteLine(customer.FullName);
            Console.WriteLine("Adresse:");
            Console.WriteLine("{0} {1}", customer.Street, customer.Number);
            Console.WriteLine("{0} {1}", customer.City, customer.Country);
            Console.WriteLine("Sidste Køb: {0:dd-MM-yyyy}", customer.LastPurchaseAt);

//TODO K5: 
            //Console.WriteLine("Tryk på F2 for at redigere kundens detaljer");
            //AddKey(ConsoleKey.F2, () =>
            //{
            //    Screen.Display(new CustomerEditScreen(customer));
            //    Quit();
            //});

            ExitOnEscape();


        }
    }
}