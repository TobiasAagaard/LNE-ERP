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
            throw new NotImplementedException();
        }
    }


