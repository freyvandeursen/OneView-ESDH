
using DTO.Customer;

namespace DataAccess.Mapper {
    internal static class ContactMapper {
        internal static DTOContact ToDTO(this Model.Contact contact) {
            return new DTOContact {
                Id = contact.Id,
                CustomerId = contact.CustomerId,
                Name = contact.Name,
                Email = contact.Email,
                Title = contact.Title,
                PhoneNumber = contact.PhoneNumber
            };
        }

        public static List<DTOContact> ToDTO(this List<Model.Contact> contacts) {
            var dtoContacts = new List<DTOContact>();
            foreach (var contact in contacts) {
                dtoContacts.Add(contact.ToDTO());
            }
            return dtoContacts;
        }

        internal static Model.Contact NewToModel(this DTOContactCreate dtoContact) {
            return new Model.Contact {
                Id = 0, 
                CustomerId = dtoContact.CustomerId,
                Name = dtoContact.Name,
                Email = dtoContact.Email,
                Title = dtoContact.Title,
                PhoneNumber = dtoContact.PhoneNumber
            };
        }

        internal static Model.Contact ApplyUpdate(this DTOContact dtoContact, Model.Contact contact) {
            contact.CustomerId = dtoContact.CustomerId;
            contact.Name = dtoContact.Name;
            contact.Email = dtoContact.Email;
            contact.Title = dtoContact.Title;
            contact.PhoneNumber = dtoContact.PhoneNumber;

            return contact;
        }
    }
}
