using DataAccess.Context;
using DataAccess.Interface;
using DataAccess.Mapper;
using DataAccess.Model;
using DTO.Customer;


namespace DataAccess.Repository {
    internal class ContactRepository : IContactRepository {

        private readonly Database _db;

        public ContactRepository(Database db) {
            _db = db;
        }
        public void AddContactToCustomer(DTOContactCreate dtoContact) {
            try {
                var contact = dtoContact.NewToModel();
                _db.Add(contact);
                _db.SaveChanges();
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                throw new Exception("Failed to add contact to customer.", ex);
            }

        }

        public bool DeleteContactForCustomer(int contactId) {
            try {
                var contact = _db.Set<Contact>().FirstOrDefault(c => c.Id == contactId);

                if (contact == null) {
                    return false;
                }

                _db.Remove(contact);
                _db.SaveChanges();

                return true;
            }
            catch (Exception ex) {
                Console.WriteLine($"Error deleting contact: {ex.Message}");
                throw new Exception("Failed to delete contact", ex);
            }
        }


        public List<DTOContact> GetContactsByCustomerId(string customerId) {
            try {
                var contacts = _db.Contacts
                    .Where(c => c.CustomerId == customerId)
                    .ToList();

                return contacts.ToDTO(); 
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                throw new Exception("Failed to retrieve contacts for customer.", ex);
            }
        }
    }
}
