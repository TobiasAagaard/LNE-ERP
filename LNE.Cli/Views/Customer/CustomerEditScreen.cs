using TECHCOOL.UI;
using ErpCli.Models;
using ErpCli.Data;

namespace ErpCli.Views;

public class CustomerEditScreen : Screen
{
    public override string Title { get; set; } = "Rediger Kunde";

    Person customer = new();

    public CustomerEditScreen(Person customer)
    {
        if (customer != null && customer.Id != 0)
        {
            Title = "Rediger " + customer.FullName;
            this.customer = customer;
        }
        this.customer = customer ?? new Person();
    }

    protected override void Draw()
    {
        ExitOnEscape();

        Form<Person> form = new();

        form.TextBox("Fornavn", nameof(Person.FirstName));
        form.TextBox("Efternavn", nameof(Person.LastName));
        form.TextBox("Email", nameof(Person.Email));
        form.TextBox("Telefon", nameof(Person.Phone));
        
        form.SearchBox("Virksomhed", nameof(Person.Company), term =>
            Database.Instance.GetAllCompanies()
                .Where(c =>
                    (c.Name ?? "").Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    c.Id.ToString().Contains(term))
                .Select(c => ($"{c.Name}", (object)c))
                .ToList());
        
        if (form.Edit(customer))
        {
            if (string.IsNullOrEmpty(customer.FirstName)
                || string.IsNullOrEmpty(customer.LastName)
                || string.IsNullOrEmpty(customer.Email)
                || string.IsNullOrEmpty(customer.Phone)
                || customer.Company == null)
            {
                Console.WriteLine("Fornavn, efternavn, email, telefon og virksomhed må ikke være tomme");
                return;
            }
            if (customer.Id != 0)
            {
                Database.Instance.UpdateCustomer(customer);
                Console.WriteLine("Kunden blev opdateret");
            }
            else
            {
                Database.Instance.CreateCustomer(customer);
                Console.WriteLine("Kunden blev oprettet");
            }
        }
        else
        {
            Console.WriteLine("Ændringerne blev ikke gemt");
        }
                
    }
}