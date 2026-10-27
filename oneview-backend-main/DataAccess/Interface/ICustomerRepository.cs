using DTO.Customer;

namespace DataAccess.Interface
{
    public interface ICustomerRepository
    {
        // Define method signatures for customer-related database operations
        void AddCustomer(DTOCustomer dtoCustomer);
        List<DTOCustomer>? GetCustomers();
        DTOCustomer? GetCustomerById(string CustomerId);
        bool UpdateCustomer(DTOCustomer dtoCustomer);
    }
}