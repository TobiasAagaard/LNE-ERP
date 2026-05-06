using ErpCli.Models;
using ErpCli.Data;
using TECHCOOL.UI;

namespace ErpCli.Views;
public class CompanyListScreen : Screen
{
    public override string Title {get;set;} = "Virksomheder";

    protected override void Draw()
    {
        ExitOnEscape();
        
        Console.CursorVisible = false;

        ListPage<Company> listPage = new();

    
        Console.WriteLine("");
        Console.WriteLine("Tryk på F1 for at oprette en ny Virksomhed");
        Console.WriteLine("Tryk på F2 for at redigere en eksisterende Virksomhed");
        Console.WriteLine("Tryk på F5 for at slette en eksisterende Virksomhed");
        Console.WriteLine("");
       
        listPage.AddKey(ConsoleKey.F1, CreateNewCompany);
        listPage.AddKey(ConsoleKey.F2, EditCompany);
        listPage.AddKey(ConsoleKey.F5, DeleteCompany);


        listPage.AddColumn("Virksomhed", nameof(Company.Name), 20);
        listPage.AddColumn("Land", nameof(Company.Country), 20);
        listPage.AddColumn("Valuta", nameof(Company.Currency), 10);

        List<Company> companies = Database.Instance.GetAllCompanies();
        foreach (Company model in companies) 
        {
            listPage.Add(model);
        }

        Company company = listPage.Select();
        if (company != null)
        {
            Screen.Display(new CompanyDetails(company));
        }
        else
        {
            Quit();
        }
    }
    void CreateNewCompany(Company _)
    {
        Company company = new Company();
        Screen.Display(new CompanyEditScreen(company)); 
    }
    
    void EditCompany(Company company)
    {
        Screen.Display(new CompanyEditScreen(company));
    }

    void DeleteCompany(Company company)
    {
        Database.Instance.DeleteCompany(company.Id);
        Screen.Clear();
        Draw();
    }
    
}