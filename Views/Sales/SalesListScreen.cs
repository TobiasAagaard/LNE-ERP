using TECHCOOL.UI;
using ErpCli.Models;
using ErpCli.Data;

namespace ErpCli.Views
{
    public class SalesListScreen : Screen
    {
        public override string Title { get; set; } = "Salgsliste";
        protected override void Draw()
        {
            ExitOnEscape();

            ListPage<SalesOrderHeader> listPage = new();
            
            Console.WriteLine();
            
            listPage.AddKey(ConsoleKey.F2, EditSalesOrderHeader);
            Console.WriteLine("Tryk F2 for at redigere en salgsordre");

            listPage.AddKey(ConsoleKey.F3, CreateNewSalesOrderHeader);
            Console.WriteLine("Tryk F3 for at oprette en ny salgsordre");

            listPage.AddKey(ConsoleKey.F5, RemoveSalesOrderHeader);
            Console.WriteLine("Tryk F5 for at slette en salgsordre");

            Console.WriteLine();
            
            listPage.AddColumn("Salgsordrenummer", nameof(SalesOrderHeader.OrderNumber), 20);
            listPage.AddColumn("Dato", nameof(SalesOrderHeader.OrderCreatedAt));
            listPage.AddColumn("Kundenummer", nameof(SalesOrderHeader.CustomerId));
            listPage.AddColumn("Navn", nameof(SalesOrderHeader.FullName));
            listPage.AddColumn("Beløb", nameof(SalesOrderHeader.OrderTotal));


            List<SalesOrderHeader> salesOrderHeaders = Database.Instance.GetSalesOrderHeaders();
            foreach (SalesOrderHeader model in salesOrderHeaders)
            {
                listPage.Add(model);
            }

            SalesOrderHeader SelectedSalesOrderHeader = listPage.Select();
            if (SelectedSalesOrderHeader != null) 
            {
                Display(new SalesDetailsScreen(SelectedSalesOrderHeader));
            }
            else
            {
                Quit();
            }
        }
        void CreateNewSalesOrderHeader(SalesOrderHeader _)
        {
            SalesOrderHeader new_SalesOrderHeader = new();
            Screen.Display(new SalesEditScreen(new_SalesOrderHeader));
        }
        void EditSalesOrderHeader(SalesOrderHeader salesOrderHeader) 
        {
            Screen.Display(new SalesEditScreen(salesOrderHeader));
        }
        void RemoveSalesOrderHeader(SalesOrderHeader salesOrderHeader) 
        {
            Database.Instance.DeleteSalesOrderHeader(salesOrderHeader.OrderNumber);
            Screen.Clear();
            Draw();
        }
    }
}