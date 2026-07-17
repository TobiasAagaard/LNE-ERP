using ErpCli.Models;
using Microsoft.Data.SqlClient;

namespace ErpCli.Data
{
    public partial class Database
    {
        /// <summary>
        /// Returns the product with the given Id, or null if no such product exists.
        /// </summary>
        public Product? GetProductById(int id)
        {
            using SqlConnection connection = GetConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT Id, ItemNumber, Name, Description, Price, Cost, Location, StockQuantity, Unit 
                                FROM Products
                                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
                return ReadProduct(reader);
            return null;
        }

        /// <summary>
        /// Returns all products from the Products table. Empty list if the table is empty.
        /// </summary>
        public List<Product> GetProducts()
        {
            List<Product> products = new();
            using SqlConnection connection = GetConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT Id, ItemNumber, Name, Description, Price, Cost, Location, StockQuantity, Unit 
                                FROM Products";
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                products.Add(ReadProduct(reader));
            return products;
        }

        /// <summary>
        /// Inserts a new product. The Id property is ignored — the database assigns it via IDENTITY.
        /// </summary>
        public void AddProduct(Product product)
        {
            using SqlConnection connection = GetConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Products (ItemNumber, Name, Description, Price, Cost, Location, StockQuantity, Unit)
                                VALUES (@itemNumber, @name, @description, @price, @cost, @location, @stockQuantity, @unit)";
            BindProductParameters(cmd, product);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Updates the product row matching product.Id with the values from the given Product.
        /// No-op if no row matches.
        /// </summary>
        public void UpdateProduct(Product product)
        {
            using SqlConnection connection = GetConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Products
                                SET Itemnumber      = @itemNumber,
                                    Name            = @name,
                                    Description     = @description,
                                    Price           = @price,
                                    Cost            = @cost,
                                    Location        = @location,
                                    StockQuantity   = @stockQuantity,
                                    Unit            = @unit
                                WHERE Id = @id";
            BindProductParameters(cmd, product);
            cmd.Parameters.AddWithValue("@id", product.Id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes the product with the given Id. No-op if no row matches.
        /// </summary>
        public void DeleteProduct(int id)
        {
            using SqlConnection connection = GetConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Products WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Maps the current row of the given reader to a Product. Column order must match
        /// the SELECT statements in this file: Id, ItemNumber, Name, Description, Price,
        /// Cost, Location, StockQuantity, Unit.
        /// </summary>
        private static Product ReadProduct(SqlDataReader reader)
        {
            return new Product
            {
                Id              = reader.GetInt32(0),
                ItemNumber      = reader.IsDBNull(1) ? null : reader.GetString(1),
                Name            = reader.IsDBNull(2) ? null : reader.GetString(2),
                Description     = reader.IsDBNull(3) ? null : reader.GetString(3),
                Price           = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                Cost            = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                Location        = reader.IsDBNull(6) ? null : reader.GetString(6),
                StockQuantity   = reader.GetDecimal(7),
                Unit            = (Unit)reader.GetInt32(8)
            };

        }

        /// <summary>
        /// Adds @itemNumber, @name, @description, @price, @cost, @location, @stockQuantity, @unit
        /// parameters to the given command from the given Product. Does NOT add @id — caller
        /// must add it for UPDATE; AddProduct does not need it (Id is IDENTITY).
        /// Nullable string/double properties are sent as DBNull.Value when null.
        /// </summary>
        private static void BindProductParameters(SqlCommand cmd, Product p)
        {
            cmd.Parameters.AddWithValue("@itemNumber", (object?)p.ItemNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@name", (object?)p.Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@description", (object?)p.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@price", (object?)p.Price ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@cost", (object?)p.Cost ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@location", (object?)p.Location ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@stockQuantity", p.StockQuantity);
            cmd.Parameters.AddWithValue("@unit", (int)p.Unit);
        }
    }
}