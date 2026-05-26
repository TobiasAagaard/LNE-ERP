using ErpCli.Models;
using Microsoft.Data.SqlClient;

namespace ErpCli.Data
{
    public partial class Database
    {
        /// <summary>
        /// Returns the order line with the given Id (with its Product hydrated via JOIN),
        /// or null if no such order line exists.
        /// </summary>
        public OrderLine? GetOrderLine(int id)
        {
            using SqlConnection connection = GetConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT  SalesOrderLines.Id, OrderNumber, Quantity,
                                        Products.Id, ItemNumber, Name, Description, Price, Cost,
                                        Location, StockQuantity, Unit
                                FROM    SalesOrderLines
                                INNER JOIN Products ON Products.Id = SalesOrderLines.ProductId
                                WHERE   SalesOrderLines.Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
                return ReadOrderLine(reader);
            return null;
        }

        /// <summary>
        /// Returns all order lines whose Id is in the given list, with each Product hydrated via JOIN.
        /// Empty list in -> empty list out (no DB round-trip).
        /// </summary>
        public List<OrderLine> GetAllOrderLine(List<int> ids)
        {
            List<OrderLine> result = new();
            if (ids.Count == 0) return result;

            using SqlConnection connection = GetConnection();
            SqlCommand cmd = connection.CreateCommand();

            List<string> paramNames = new();
            for (int i = 0; i < ids.Count; i++)
            {
                string name = "@id" + i;
                paramNames.Add(name);
                cmd.Parameters.AddWithValue(name, ids[i]);
            }

            cmd.CommandText = $@"SELECT  SalesOrderLines.Id, OrderNumber, Quantity,
                                         Products.Id, ItemNumber, Name, Description, Price, Cost,
                                         Location, StockQuantity, Unit
                                 FROM    SalesOrderLines
                                 INNER JOIN Products ON Products.Id = SalesOrderLines.ProductId
                                 WHERE   SalesOrderLines.Id IN ({string.Join(",", paramNames)})";

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(ReadOrderLine(reader));
            return result;
        }

        /// <summary>
        /// Inserts a new order line under the given orderNumber. The Id property is ignored
        /// — the database assigns it via IDENTITY and it is written back onto orderLine.Id.
        /// orderLine.Product must be non-null; its Id is used as ProductId.
        /// </summary>
        public void AddOrderLine(OrderLine orderLine, int orderNumber)
        {
            using SqlConnection connection = GetConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO SalesOrderLines (OrderNumber, ProductId, Quantity)
                                VALUES (@orderNumber, @productId, @quantity);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            cmd.Parameters.AddWithValue("@orderNumber", orderNumber);
            cmd.Parameters.AddWithValue("@productId", orderLine.Product!.Id);
            cmd.Parameters.AddWithValue("@quantity", orderLine.Quantity);
            orderLine.Id = (int)cmd.ExecuteScalar();
        }

        /// <summary>
        /// Updates the order line row matching editOrderLine.Id with the given Product and Quantity.
        /// OrderNumber is intentionally not updated — a line does not move between parent orders.
        /// No-op if no row matches.
        /// </summary>
        public void UpdateOrderLine(OrderLine editOrderLine)
        {
            using SqlConnection connection = GetConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE SalesOrderLines
                                SET ProductId = @productId,
                                    Quantity  = @quantity
                                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@productId", editOrderLine.Product!.Id);
            cmd.Parameters.AddWithValue("@quantity", editOrderLine.Quantity);
            cmd.Parameters.AddWithValue("@id", editOrderLine.Id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes the order line with the given Id. No-op if no row matches.
        /// </summary>
        public void DeleteOrderLine(int id)
        {
            using SqlConnection connection = GetConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM SalesOrderLines WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Returns all order lines belonging to the given OrderNumber, with each Product hydrated via JOIN.
        /// Empty list if the parent header has no lines.
        /// </summary>
        public List<OrderLine> GetOrderLinesByOrderNumber(int orderNumber)
        {
            List<OrderLine> result = new();
            using SqlConnection connection = GetConnection();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT  SalesOrderLines.Id, OrderNumber, Quantity,
                                Products.Id, ItemNumber, Name, Description, Price, Cost,
                                Location, StockQuantity, Unit
                        FROM    SalesOrderLines
                        INNER JOIN Products ON Products.Id = SalesOrderLines.ProductId
                        WHERE   OrderNumber = @orderNumber";
            cmd.Parameters.AddWithValue("@orderNumber", orderNumber);

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(ReadOrderLine(reader));
            return result;
        }

        /// <summary>
        /// Maps the current row of the given reader to an OrderLine with its Product hydrated.
        /// Column order must match the SELECT statements in this file:
        /// SalesOrderLines.Id, OrderNumber, Quantity, Products.Id, ItemNumber, Name, Description,
        /// Price, Cost, Location, StockQuantity, Unit.
        /// </summary>
        private static OrderLine ReadOrderLine(SqlDataReader reader)
        {
            return new OrderLine
            {
                Id          = reader.GetInt32(0),
                Quantity    = reader.GetDecimal(2),
                Product     = new Product
                {
                    Id              = reader.GetInt32(3),
                    ItemNumber      = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Name            = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Description     = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Price           = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
                    Cost            = reader.IsDBNull(8) ? null : reader.GetDecimal(8),
                    Location        = reader.IsDBNull(9) ? null : reader.GetString(9),
                    StockQuantity   = reader.GetDecimal(10),
                    Unit            = (Unit)reader.GetInt32(11)
                }
            };
        }
    }
}