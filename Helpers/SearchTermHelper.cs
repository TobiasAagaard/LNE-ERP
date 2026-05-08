namespace ErpCli.Helpers;

public static class SearchHelper
{
    public static bool MatchSearchTerm(string? value, string term)
    {
        return (value ?? "").Contains(term, StringComparison.OrdinalIgnoreCase);
    }
}