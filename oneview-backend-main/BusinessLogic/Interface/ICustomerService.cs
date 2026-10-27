using DTO.Customer;

namespace BusinessLogic.Interface {

    public interface ICustomerService {
        DTOCustomer? GetCustomerById(string CustomerId);
        List<DTOCustomer>? GetCustomers();
        void AddCustomer(DTOCustomer dtoCustomer);
        void DeactivateCustomer(string CustomerId, DateTime? DeactivationDate);
    }
}