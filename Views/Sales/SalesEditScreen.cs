using TECHCOOL.UI;
using ErpCli.Models;
using ErpCli.Helpers;
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

            form.SearchBox("Kunde", nameof(salesOrderHeader.customer), term =>
                Database.Instance.GetAllCustomers()
                    .Where(c =>
                        SearchHelper.MatchSearchTerm(c.FullName, term) ||
                        SearchHelper.MatchSearchTerm(c.Email, term) ||
                        SearchHelper.MatchSearchTerm(c.CustomerId.ToString(), term) ||
                        SearchHelper.MatchSearchTerm(c.Phone, term))
                    .Select(c => ($"{c.CustomerId} - {c.FullName}", (object)c))
                    .ToList());
            form.SelectBox("Status", nameof(salesOrderHeader.Status));
            foreach (var s in Enum.GetValues<SalesOrderHeader.OrderStatus>())
            {
                form.AddOption("Status", s.ToString(), s);
            }

            if (form.Edit(salesOrderHeader))
            {
                salesOrderHeader.CustomerId = salesOrderHeader.customer?.CustomerId ?? 0;

                if (salesOrderHeader.OrderNumber != 0)
                {
                    Database.Instance.UpdateSalesOrderHeader(salesOrderHeader);
                }
                else
                {
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
