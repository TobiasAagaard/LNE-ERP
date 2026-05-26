using Microsoft.Data.SqlClient;
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
        } catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) 
        {
            //Unique constraint, due to concurrent insert race condition. Try to find the address again.

            result = find.ExecuteScalar();
            if (result is int retryId) return retryId;
            if (result is not null) return Convert.ToInt32(result);
            throw new InvalidOperationException("Failed to retrieve address ID after concurrent insert.");
        }
    }

    public int DeleteAddressIfNotReferenced(int addressId, SqlConnection connection, SqlTransaction transaction)
    {
        using SqlCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = @"DELETE FROM Addresses
                                WHERE Id = @id
                                AND NOT EXISTS (SELECT 1 FROM Companies WHERE AddressId = @id)
                                AND NOT EXISTS (SELECT 1 FROM Persons WHERE AddressId = @id)";
        command.Parameters.AddWithValue("@id", addressId);
        return command.ExecuteNonQuery();
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