using DataAccess.Context;
using DataAccess.Interface;
using DataAccess.Mapper;
using DataAccess.Model;
using DTO.Customer;
using System.Data.Common;

namespace DataAccess.Repository {
    internal class CustomerRepository : ICustomerRepository {

        private readonly Database _db;

        public CustomerRepository(Database db) {
            _db = db;
        }

        public void AddCustomer(DTOCustomer dtoCustomer) {
            try {
                var customer = dtoCustomer.NewToModel();
                _db.Add(customer);
                _db.SaveChanges();
            }
            catch (Exception ex) {
                Console.WriteLine($"Error adding customer: {ex.Message}");
                throw new Exception("Failed to add customer", ex);
            }
        }
        public DTOCustomer? GetCustomerById(string CustomerId) {
            try {
                var customer = (from c in _db.Set<Customer>()
                                where c.CustomerId == CustomerId
                                select c).FirstOrDefault();

                // Not found should be handled by the caller.
                return customer == null ? null : CustomerMapper.ToDTO(customer);
            }
            catch (Exception ex) {
                Console.WriteLine($"Error retrieving customer by ID: {ex.Message}");
                throw new Exception("Failed to retrieve customer", ex);
            }
        }

        public List<DTOCustomer>? GetCustomers() {
            try {
                var customers = (
                    from c in _db.Customers
                    select c).ToList();

                // Not found should be handled by the caller.
                return customers == null || customers.Count == 0 ? null : CustomerMapper.ToDTO(customers);
            }
            catch (DbException ex) {
                Console.WriteLine($"Database error retrieving customers: {ex.Message}");
                throw new Exception("Failed to retrieve customers due to a database error.", ex);
            }
        }
        public bool UpdateCustomer(DTOCustomer dtoCustomer) {
            try {
                var customer = (from c in _db.Set<Customer>()
                                where c.CustomerId == dtoCustomer.CustomerId
                                select c).FirstOrDefault();

                // Not found should be handled by the caller.
                if (customer == null) {
                    return false;
                }

                customer = CustomerMapper.DTOToModel(dtoCustomer, customer);
                _db.Update(customer);
                _db.SaveChanges();
                return true;

            }
            catch (Exception ex) {
                Console.WriteLine($"Error updating customer: {ex.Message}");
                throw new Exception("Failed to update customer", ex);
            }
        }
    }
}