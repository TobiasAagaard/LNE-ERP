using TECHCOOL.UI;
using ErpCli.Models;
using ErpCli.Data;


namespace ErpCli.Views
{
    class CustomerEditScreen : Screen
    {
        public override string Title { get; set; } = "Rediger Kunde";
        Customer customer = new();

        public CustomerEditScreen(Customer customer)
        {
            if (customer.CustomerId != 0)
            {
                Title = $"Rediger Kunde: {customer.FullName}";
            }
            this.customer = customer ?? new Customer();
        }

        protected override void Draw()
        {
            ExitOnEscape();
            Form<Customer> form = new();

            form.TextBox("Fornavn", nameof(customer.FirstName));
            form.TextBox("Efternavn", nameof(customer.LastName));
            form.TextBox("Vej", nameof(customer.Street));
            form.TextBox("Husnummer", nameof(customer.Number));
            form.TextBox("Postnummer", nameof(customer.PostalCode));
            form.TextBox("By", nameof(customer.City));
            form.TextBox("Telefon", nameof(customer.Phone));
            form.TextBox("Email", nameof(customer.Email));
            if (form.Edit(customer))
            {
                if (customer.CustomerId != 0)
                {
                    Database.Instance.UpdateCustomer(customer);
                }
                else
                {
                    Database.Instance.AddCustomer(customer);
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