using TECHCOOL.UI;
using ErpCli.Models;
using ErpCli.Data;
using ErpCli.Helpers;

namespace ErpCli.Views;

public class CustomerListScreen : Screen
{
    public override string Title { get; set; } = "Kunde Personer";

    protected override void Draw()
    {
        ExitOnEscape();

        Console.CursorVisible = false;

        ListPage<Person> listPage = new();

        Console.WriteLine("");
        Console.WriteLine("Tryk på F1 for at oprette en ny Kunde Person");
        Console.WriteLine("Tryk på F2 for at redigere en eksisterende Kunde Person");
        Console.WriteLine("Tryk på F5 for at slette en eksisterende Kunde Person");
        Console.WriteLine("");

        listPage.AddKey(ConsoleKey.F1, CreateNewCustomer);
        listPage.AddKey(ConsoleKey.F2, EditCustomer);
        listPage.AddKey(ConsoleKey.F5, RemoveCustomer);

        listPage.AddColumn("Navn", nameof(Person.FullName), 25);
        listPage.AddColumn("Telefon", nameof(Person.Phone), 15);
        listPage.AddColumn("Email", nameof(Person.Email), 25);
        listPage.AddColumn("Firma", nameof(Person.CompanyName), 25);

        List<Person> customers;
       
        customers = Database.Instance.GetAllCustomers();
        
        foreach (Person model in customers)
        {
            listPage.Add(model);
        }

        Person customer = listPage.Select();

        if (customer != null)
        {
            Screen.Display(new CustomerDetailsScreen(customer));
        }
        else 
        {
            Quit();
        }

        void CreateNewCustomer(Person _)
        {
            Person customer = new Person();
            Screen.Display(new CustomerEditScreen(customer));
        }

        void EditCustomer(Person customer)
        {
            Screen.Display(new CustomerEditScreen(customer));
        }

        void RemoveCustomer(Person customer)
        {
            Database.Instance.DeleteCustomerById(customer.Id);
            Screen.Clear();
            Draw(); 
        }
        
        
    }

}