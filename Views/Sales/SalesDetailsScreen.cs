using TECHCOOL.UI;
using ErpCli.Models;
using ErpCli.Data;

namespace ErpCli.Views
{
    public class SalesDetailsScreen : Screen
    {
        public override string Title { get; set; } = "Salgsordredetaljer";
        SalesOrderHeader header = new();

        public SalesDetailsScreen(SalesOrderHeader salesOrderHeader)
        {
            Title = "Detaljer for " + salesOrderHeader.OrderNumber;
            header = salesOrderHeader;
        }

        protected override void Draw()
        {
            ExitOnEscape();
            Customer? customer = Database.Instance.GetCustomerById(header.CustomerId);

            Console.WriteLine("Tryk F2 for at rediger ordrehovedet");
            Console.WriteLine("Tryk F3 for at redigere en ordrelinje");
            Console.WriteLine("Tryk F4 for at oprette en ny ordrelinje");
            Console.WriteLine("Tryk F5 for at slette en ordrelinje");

            AddKey(ConsoleKey.F2, () => {
                Screen.Display(new SalesEditScreen(header));
            });
            Console.WriteLine();

            Console.WriteLine($"Ordrenummer: {header.OrderNumber}");
            Console.WriteLine($"Dato oprettet: {header.OrderCreatedAt}");
            Console.WriteLine($"Dato færdig: {header.OrderCompletedAt}");
            Console.WriteLine($"Kundenummer: {header.CustomerId}");
            Console.WriteLine($"Navn: {header.FullName}");

            ListPage<OrderLine> listPage = new();
            
            Console.WriteLine();
            
            listPage.AddKey(ConsoleKey.F3, EditOrderLine);
            listPage.AddKey(ConsoleKey.F4, CreateNewOrderLine);
            listPage.AddKey(ConsoleKey.F5, RemoveOrderLine);
            

            Console.WriteLine();
            
            listPage.AddColumn("Produkt", nameof(OrderLine.Name), 20);
            listPage.AddColumn("Antal", nameof(OrderLine.Quantity));


            List<OrderLine> orderLineList = Database.Instance.GetOrderLinesByOrderNumber(header.OrderNumber);
            foreach (OrderLine model in orderLineList)
            {
                listPage.Add(model);
            }

            OrderLine SelectedOrderLine = listPage.Select();
            if (SelectedOrderLine != null) 
            {
                Display(new OrderLineEditScreen(SelectedOrderLine, header.OrderNumber));
            }
            else
            {
                Quit();
            }
        }
        void CreateNewOrderLine(OrderLine _)
        {
            OrderLine new_OrderLine = new();
            Screen.Display(new OrderLineEditScreen(new_OrderLine, header.OrderNumber));
        }
        void EditOrderLine(OrderLine orderLine) 
        {
            Screen.Display(new OrderLineEditScreen(orderLine, header.OrderNumber));
        }
        void RemoveOrderLine(OrderLine orderLine) 
        {
            Database.Instance.DeleteOrderLine(orderLine.Id);
            Screen.Clear();
            Draw();
        }
    }
}