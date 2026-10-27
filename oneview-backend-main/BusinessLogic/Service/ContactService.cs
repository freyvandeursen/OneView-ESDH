using BusinessLogic.Interface;
using DataAccess.Interface;
using DTO.Customer;

namespace BusinessLogic.Service {
    public class ContactService : IContactService {
        private readonly IContactRepository _contactRepository;
        private readonly ICustomerRepository _customerRepository;

        public ContactService(IContactRepository contactRepository, ICustomerRepository customerRepository) {
            _contactRepository = contactRepository;
            _customerRepository = customerRepository;
        }
        public void AddContactToCustomer(DTOContactCreate dtoContact) {
            if(_customerRepository.GetCustomerById(dtoContact.CustomerId) == null) {
                throw new ArgumentException($"Customer with ID {dtoContact.CustomerId} doesn't exist.");
            }
            _contactRepository.AddContactToCustomer(dtoContact);
        }

        public bool DeleteContactForCustomer(int contactId) {
            return !_contactRepository.DeleteContactForCustomer(contactId)
                ? throw new ArgumentException($"Contact with ID {contactId} doesn't exist.")
                : true;
        }

        //return empty list if no contacts found
        public List<DTOContact>? GetContactsByCustomerId(string customerId) {
            return _contactRepository.GetContactsByCustomerId(customerId) ?? new List<DTOContact>();
        }
    }
}
