using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ErpCli.Data
{
    public partial class Database
    {
        /// <summary>
        /// Application configuration loaded from appsettings.Local.json.
        /// Built once at type initialization and reused across all GetConnection calls.
        /// </summary>
        private static readonly IConfiguration _config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Local.json", optional: false, reloadOnChange: false)
            .Build();

        /// <summary>
        /// Opens and returns a new SqlConnection using credentials read from
        /// appsettings.Local.json (Database section). Caller owns the connection
        /// and is responsible for disposing it (typically via 'using').
        /// </summary>
        private SqlConnection GetConnection()
        {
            IConfigurationSection db = _config.GetSection("Database");

            SqlConnectionStringBuilder builder = new()
            {
                DataSource = db["DataSource"]!,
                UserID = db["UserId"]!,
                Password = db["Password"]!,
                InitialCatalog = db["InitialCatalog"]!,
                TrustServerCertificate = true
            };
            SqlConnection connection = new SqlConnection(builder.ToString());
            connection.Open();
            return connection;
        }
    }
}