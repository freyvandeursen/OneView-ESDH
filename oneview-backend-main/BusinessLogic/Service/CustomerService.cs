using BusinessLogic.Interface;
using DataAccess.Interface;
using DTO.Customer;

namespace BusinessLogic.Service {
    public class CustomerService : ICustomerService {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository) {
            _customerRepository = customerRepository;
        }

        public DTOCustomer? GetCustomerById(string CustomerId) {
            return _customerRepository.GetCustomerById(CustomerId);
        }

        public List<DTOCustomer>? GetCustomers() {
            return _customerRepository.GetCustomers();
        }

        public void AddCustomer(DTOCustomer dtoCustomer) {

            if (_customerRepository.GetCustomerById(dtoCustomer.CustomerId) != null) {
                throw new ArgumentException($"Customer with ID {dtoCustomer.CustomerId} already exists.");
            }
            _customerRepository.AddCustomer(dtoCustomer);
        }

        public void DeactivateCustomer(string CustomerId, DateTime? DeactivationDate) {
            var customer = _customerRepository.GetCustomerById(CustomerId) ?? throw new Exception($"Customer with ID {CustomerId} not found.");
            customer.DeactivationDate = DeactivationDate;
            if (!_customerRepository.UpdateCustomer(customer)) {
                throw new Exception($"Failed to deactivate customer with ID {CustomerId}.");
            }

        }
    }
}