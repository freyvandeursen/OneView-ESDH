namespace BusinessLogic.Interface {
    public interface IContactService {
        void AddContactToCustomer(DTO.Customer.DTOContactCreate dtoContact);
        List<DTO.Customer.DTOContact>? GetContactsByCustomerId(string customerId);
        bool DeleteContactForCustomer(int contactId);
    }
}
