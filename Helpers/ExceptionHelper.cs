namespace ErpCli.Helpers
{
    public static class ExceptionHelper
    {
        public static void HandleException(Exception ex, string contextMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{contextMessage}: {ex.Message}");
            Console.ResetColor();
        }
    }
}