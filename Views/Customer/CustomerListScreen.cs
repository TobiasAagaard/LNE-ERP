using ErpCli.Models;
using ErpCli.Data;
using TECHCOOL.UI;

namespace ErpCli.Views;

public class CustomerListScreen : Screen
{
    public override string Title { get; set; } = "Kunder";

    protected override void Draw()
    {
        ExitOnEscape();

        Console.CursorVisible = false;

        ListPage<Customer> listPage = new();


        Console.WriteLine("");
        Console.WriteLine("Tryk på F1 for at oprette en ny kunde");
        Console.WriteLine("Tryk på F2 for at redigere en eksisterende kunde");
        Console.WriteLine("Tryk på F5 for at slette en eksisterende kunde");
        Console.WriteLine("");

        listPage.AddKey(ConsoleKey.F1, CreateNewCustomer);
        listPage.AddKey(ConsoleKey.F2, EditCustomer);
        listPage.AddKey(ConsoleKey.F5, RemoveCustomer);

        listPage.AddColumn("Kundenummer", nameof(Customer.CustomerId), 12);
        listPage.AddColumn("Navn", nameof(Customer.FullName), 30);
        listPage.AddColumn("Telefon", nameof(Customer.Phone), 15);
        listPage.AddColumn("Email", nameof(Customer.Email), 30);

        List<Customer> customers = Database.Instance.GetAllCustomers();
        foreach (Customer model in customers)
        {
            listPage.Add(model);
        }

        Customer customer = listPage.Select();
        if (customer != null)
        {
            Screen.Display(new CustomerDetailsScreen(customer));
        }
        else
        {
            Quit();
        }
    }


    void CreateNewCustomer(Customer _)
    {
        Customer customer = new Customer();
        Screen.Display(new CustomerEditScreen(customer));
    }

    void EditCustomer(Customer customer)
    {
        Screen.Display(new CustomerEditScreen(customer));
    }
    void RemoveCustomer(Customer customer)
    {
        Database.Instance.DeleteCustomerById(customer.CustomerId);
        Console.Clear();
        Draw();
    }
  

}