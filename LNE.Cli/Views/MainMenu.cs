using TECHCOOL.UI;
namespace ErpCli.Views
{
    public class MainMenu : Screen
    {
        public override string Title { get; set; } = "LNE Security ";
        protected override void Draw()
        {
            Console.CursorVisible = false;

            Menu menu = new Menu();

            menu.Add(new CompanyListScreen());
            menu.Add(new ProductListPage());
            menu.Add(new SalesListScreen());
            menu.Add(new CustomerListScreen());

            Display(menu);
        }
    }
}