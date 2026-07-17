using TECHCOOL.UI;
using ErpCli.Models;
using ErpCli.Data;

namespace ErpCli.Views
{
    public class ProductDetails : Screen
    {
        public override string Title { get; set; } = "Produktdetaljer";
        Product product = new();

        public ProductDetails(Product product)
        {
            Title = "Detaljer for " + product.Name;
            this.product = product;
        }

        protected override void Draw()
        {
            ExitOnEscape();

            Console.WriteLine($"Varenummer: {product.ItemNumber}");
            Console.WriteLine($"Navn: {product.Name}");
            Console.WriteLine($"Beskrivelse: {product.Description}");
            Console.WriteLine($"Salgspris: {product.Price}");
            Console.WriteLine($"Indkøbspris: {product.Cost}");
            Console.WriteLine($"Lokation: {product.Location}");
            Console.WriteLine($"Lagerantal: {product.StockQuantity}");
            Console.WriteLine($"Avance i procent: {product.ProfitPercent}%");
            Console.WriteLine($"Avance i kr: {product.Price - product.Cost}");

            Console.WriteLine("Tryk F2 for at redigere");
            AddKey(ConsoleKey.F2, () => {
                Screen.Display( new ProductEditor(product) );
            });
        }
    }
}