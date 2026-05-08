using TECHCOOL.UI;
using ErpCli.Models;
using ErpCli.Helpers;
using ErpCli.Data;

namespace ErpCli.Views
{
    public class OrderLineEditScreen : Screen
    {
        public override string Title { get; set; } = "Salgsordre";
        OrderLine orderLine = new();
        int headerId;

        public OrderLineEditScreen(OrderLine orderLine, int headerId)
        {
            if (orderLine.Id > 0)
            {
                Title = "Redigerer for " + orderLine.Id;
            }
            else
            {
                Title = "Opret ny salgsordre";
            }
            this.orderLine = orderLine;
            this.headerId = headerId;
        }

        protected override void Draw()
        {
            ExitOnEscape();

            Form<OrderLine> form = new();

            form.SearchBox("Produkt", nameof(orderLine.Product), term =>
                Database.Instance.GetProducts()
                    .Where(p =>
                        SearchHelper.MatchSearchTerm(p.Name, term) ||
                        SearchHelper.MatchSearchTerm(p.ItemNumber, term) ||
                        SearchHelper.MatchSearchTerm(p.Id.ToString(), term))
                    .Select(p => ($"{p.Name} - {p.ItemNumber}", (object)p))
                    .ToList());
            form.DoubleBox("Antal", nameof(orderLine.Quantity));

            if (form.Edit(orderLine))
            {
                if (orderLine.Id != 0)
                {
                    Database.Instance.UpdateOrderLine(orderLine);
                }
                else
                {
                    Database.Instance.AddOrderLine(orderLine, headerId);
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
