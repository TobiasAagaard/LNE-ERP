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

            // Pick the company that gets billed (the debtor). Required.
            form.SearchBox("Virksomhed", nameof(salesOrderHeader.Company), term =>
                Database.Instance.GetAllCompanies()
                    .Where(c =>
                        (c.Name ?? "").Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        c.Id.ToString().Contains(term))
                    .Select(c => ($"{c.Name} (#{c.Id})", (object)c))
                    .ToList());

            // Pick the contact person who placed the order. Optional, and limited to
            // the chosen company's contacts (evaluated lazily when the user searches).
            form.SearchBox("Kontaktperson", nameof(salesOrderHeader.ContactPerson), term =>
                Database.Instance.GetAllCustomers()
                    .Where(p => salesOrderHeader.Company == null || p.CompanyId == salesOrderHeader.Company.Id)
                    .Where(p =>
                        (p.FullName ?? "").Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        p.Id.ToString().Contains(term))
                    .Select(p => ($"{p.FullName} (#{p.Id})", (object)p))
                    .ToList());

            form.SelectBox("Status", nameof(salesOrderHeader.Status));

            foreach (var s in Enum.GetValues<SalesOrderHeader.OrderStatus>())
            {
                form.AddOption("Status", s.ToString(), s);
            }

            if (form.Edit(salesOrderHeader))
            {
                if (salesOrderHeader.Company == null)
                {
                    Console.WriteLine("Der skal vælges en virksomhed for ordren");
                    return;
                }

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
