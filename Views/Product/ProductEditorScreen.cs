using TECHCOOL.UI;
using ErpCli.Models;
using ErpCli.Data;

namespace ErpCli.Views
{
    public class ProductEditor : Screen
    {
        public override string Title { get; set; } = "Produkt";
        Product product = new();

        public ProductEditor(Product product)
        {
            if (product.Id > 0)
            {
                Title = "Redigerer for " + product.Name;
            }
            else
            {
                Title = "Opret nyt produkt";
            }
            this.product = product;
        }

        protected override void Draw()
        {
            ExitOnEscape();
            Form<Product> form = new();

            form.TextBox("Varenummer", nameof(product.ItemNumber));
            form.TextBox("Navn", nameof(product.Name));
            form.TextBox("Beskrivelse", nameof(product.Description));
            form.DoubleBox("Salgspris", nameof(product.Price));
            form.DoubleBox("Indkøbspris", nameof(product.Cost));
            form.TextBox("Lokation", nameof(product.Location));
            form.DoubleBox("Antal på lager", nameof(product.StockQuantity));
            form.SelectBox("Enhed", nameof(product.Unit));
            form.AddOption("Enhed", "Styk", Unit.Styk);
            form.AddOption("Enhed", "Timer", Unit.Timer);
            form.AddOption("Enhed", "Meter", Unit.Meter);

            if (form.Edit(product)) 
            {
                if (string.IsNullOrEmpty(product.ItemNumber)
                    || string.IsNullOrEmpty(product.Name)
                    || string.IsNullOrEmpty(product.Description)
                    || string.IsNullOrEmpty(product.Location)
                    || product.Price <= 0 
                    || product.Cost < 0
                    || product.StockQuantity < 0)
                {
                    Console.WriteLine("Felterne må ikke være tomme, og priser/antal skal være positive!");
                    return;
                }
                if (product.Id != 0)
                {
                    Database.Instance.UpdateProduct(product);
                }
                else
                {
                    Database.Instance.AddProduct(product);
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