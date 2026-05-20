using System.Data.SqlClient;
using ErpCli.Models;

namespace ErpCli.Data;

public partial class Database
{
    public int GetOrCreateAddressId(Address address, SqlConnection connection, SqlTransaction transaction)
    {
        using SqlCommand find = connection.CreateCommand();
        find.Transaction = transaction;
        find.CommandText = @"SELECT Id FROM Addresses
                            WHERE Street = @street
                            AND Number = @number
                            AND PostalCode = @postalCode
                            AND City = @city
                            AND Country = @country";
        BindAddressParameters(find, address);

        object? result = find.ExecuteScalar();
        if (result is int id) return id;
        if (result is not null) return Convert.ToInt32(result);
        try {
        using SqlCommand insert = connection.CreateCommand();
        insert.Transaction = transaction;
        insert.CommandText = @"INSERT INTO Addresses (Street, Number, PostalCode, City, Country)
                                OUTPUT INSERTED.Id
                                VALUES (@street, @number, @postalCode, @city, @country) ";
        BindAddressParameters(insert, address);
        
        object? insertResult = insert.ExecuteScalar();
        if (insertResult is null)
        {
            throw new InvalidOperationException("Failed to insert address and retrieve new ID.");
        }
        return Convert.ToInt32(insertResult);
        } catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) // Unique constraint violation
        {
            //Unique constraint violation, likely due to a concurrent insert. Try to find the address again.

            result = find.ExecuteScalar();
            if (result is int retryId) return retryId;
            if (result is not null) return Convert.ToInt32(result);
            throw new InvalidOperationException("Failed to retrieve address ID after concurrent insert.");
        }
    }

    private static void BindAddressParameters(SqlCommand cmd, Address address)
    {
        cmd.Parameters.AddWithValue("@street", address.Street);
        cmd.Parameters.AddWithValue("@number", address.Number);
        cmd.Parameters.AddWithValue("@postalCode", address.PostalCode);
        cmd.Parameters.AddWithValue("@city", address.City);
        cmd.Parameters.AddWithValue("@country", address.Country);
    }
}