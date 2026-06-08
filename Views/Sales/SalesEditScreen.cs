using TECHCOOL.UI;
using ErpCli.Models;
using ErpCli.Data;

namespace ErpCli.Views
{
    public class SalesEditScreen : Screen
    {
        public override string Title { get; set; } = "Salgsordre";
        SalesOrderHeader salesOrderHeader = new();

        public SalesEditScreen(SalesOrderHeader salesOrderHeader)
        {
            if (salesOrderHeader.OrderNumber > 0)
            {
                Title = "Redigerer for " + salesOrderHeader.OrderNumber;
            }
            else
            {
                Title = "Opret ny salgsordre";
            }
            this.salesOrderHeader = salesOrderHeader;
        }

        protected override void Draw()
        {
            ExitOnEscape();

            Form<SalesOrderHeader> form = new();

            form.SelectBox("Status", nameof(salesOrderHeader.Status));
            
            foreach (var s in Enum.GetValues<SalesOrderHeader.OrderStatus>())
            {
                form.AddOption("Status", s.ToString(), s);
            }

            if (form.Edit(salesOrderHeader))
            {
                
                if (salesOrderHeader.OrderNumber != 0)
                {
                    if (salesOrderHeader.Status == SalesOrderHeader.OrderStatus.Færdig)
                        salesOrderHeader.OrderCompletedAt = DateTime.Now;

                    Database.Instance.UpdateSalesOrderHeader(salesOrderHeader);
                }
                else
                {
                    if (salesOrderHeader.Status == SalesOrderHeader.OrderStatus.Færdig)
                        salesOrderHeader.OrderCompletedAt = DateTime.Now;
                        
                    salesOrderHeader.OrderCreatedAt = DateTime.Now;
                    Database.Instance.AddSalesOrderHeader(salesOrderHeader);
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
