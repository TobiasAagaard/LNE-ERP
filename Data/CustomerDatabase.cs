using ErpCli.Models;

namespace ErpCli.Data
{
    public partial class Database
    {
        List<Customer> Customers = new List<Customer>()
        {
            new Customer { Id = 1, CustomerId = 1001, LastPurchaseAt = new DateTime(2026, 4, 20), FirstName = "Julius", LastName = "Cæsar", Phone = "66998822", Email = "ROME69@empire.com", Address = new Address {Street = "Krigsvej", Number = "1", PostalCode = "1234", City = "Rom", Country = "Italien"}},
            new Customer { Id = 2, CustomerId = 1002, LastPurchaseAt = new DateTime(2026, 4, 26), FirstName = "Harald", LastName = "Blåtand", Phone = "45454545", Email = "DANMARK45@danemail.com", Address = new Address {Street = "Vikingevej", Number = "45", PostalCode = "9999", City = "Aalborg", Country = "Danmark"}}
        };

        public Customer? GetCustomerById(int id)
        {
            for (int i = 0; i < Customers.Count; i++)
            {
                Customer customer = Customers[i];
                if (id == customer.CustomerId)
                {
                    return customer;
                }
            }
                return null;
        }

        public List<Customer> GetAllCustomers()
        {
            List<Customer> customerCopy = new List<Customer>();
            customerCopy.AddRange(Customers);
            return customerCopy;
        }

        public void AddCustomer(Customer customer)
        {
            if (customer.Id != 0) return;
            customer.Id = Customers.Count + 1;
            customer.CustomerId = 1000 + Customers.Count + 1;
            Customers.Add(customer);
        }

        public void UpdateCustomer(Customer updatedCustomer)
        {
            for (int i = 0; i < Customers.Count; i++)
            {
                Customer customer = Customers[i];
                if (customer.CustomerId == updatedCustomer.CustomerId)
                {
                    Customers[i] = updatedCustomer;
                }
            }
        }
        public void DeleteCustomerById(int id)
        {
            for (int i = 0; i < Customers.Count; i++)
            {
                Customer customer = Customers[i];
                if (customer.CustomerId == id)
                {
                    Customers.RemoveAt(i);
                    break;
                }
            }
        }
    }

}
