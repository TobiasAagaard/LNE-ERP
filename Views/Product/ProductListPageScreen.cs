using TECHCOOL.UI;
using ErpCli.Models;
using ErpCli.Data;

namespace ErpCli.Views
{
    public class ProductListPage : Screen
    {
        public override string Title { get; set; } = "Produktliste";

        protected override void Draw()
        {
            ExitOnEscape();

            ListPage<Product> listPage = new();

            Console.WriteLine();
            
            listPage.AddKey(ConsoleKey.F2, EditProduct);
            Console.WriteLine("Tryk F2 for at redigere et produkt");

            listPage.AddKey(ConsoleKey.F3, CreateNewProduct);
            Console.WriteLine("Tryk F3 for at oprette et nyt produkt");

            listPage.AddKey(ConsoleKey.F5, RemoveProduct);
            Console.WriteLine("Tryk F5 for at slette et produkt");

            Console.WriteLine();

            listPage.AddColumn("Varenummer", nameof(Product.ItemNumber));
            listPage.AddColumn("Navn", nameof(Product.Name));
            listPage.AddColumn("Lagerantal", nameof(Product.StockQuantity));
            listPage.AddColumn("Indkøbspris", nameof(Product.Cost));
            listPage.AddColumn("Salgspris", nameof(Product.Price));
            listPage.AddColumn("Avance i procent", nameof(Product.ProfitPercent), 20);


            List<Product> products = Database.Instance.GetProducts();
            foreach (Product model in products)
            {
                listPage.Add(model);
            }

            Product product = listPage.Select();
            if (product != null) 
            {
                Display(new ProductDetails(product));
            }
            else
            {
                Quit();
            }
        }
        void CreateNewProduct(Product _)
        {
            Product new_product = new();
            Screen.Display(new ProductEditor(new_product));
        }
        void EditProduct(Product product) 
        {
            Screen.Display(new ProductEditor(product));
        }
        void RemoveProduct(Product product) 
        {
            Database.Instance.DeleteProduct(product.Id);
            Screen.Clear();
            Draw();
        }
    }
}