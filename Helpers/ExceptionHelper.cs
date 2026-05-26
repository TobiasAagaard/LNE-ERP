namespace ErpCli.Helpers
{
    public static class ExceptionHelper
    {
        public static void ExceptionText(Exception ex, string contextMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine($"{contextMessage}");
            Console.WriteLine();
            Console.WriteLine($"Fejlbesked: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("Klik på en vilkårlig tast for at gå tilbage til hovedmenuen");
            Console.ResetColor();
        }
    }
}