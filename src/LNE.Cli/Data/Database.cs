namespace ErpCli.Data
{
    public partial class Database
    {
        public static Database Instance { get; private set; } = new Database();
    }
}