using DTO.Customer;

namespace DataAccess.Interface {
    public interface IContactRepository {
        void AddContactToCustomer(DTOContactCreate dtoContact);
        List<DTOContact>? GetContactsByCustomerId(string customerId);
        bool DeleteContactForCustomer(int contactId);
    }
}
