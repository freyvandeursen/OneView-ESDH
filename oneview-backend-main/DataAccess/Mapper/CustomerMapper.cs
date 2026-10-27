using DTO.Customer;
namespace DataAccess.Mapper {
    internal static class CustomerMapper {
        internal static DTOCustomer ToDTO(this Model.Customer customer) {
            return new DTOCustomer {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Address = customer.Address,
                Email = customer.Email,
                VATNumber = customer.VATNumber,
                PhoneNumber = customer.PhoneNumber,
                DeactivationDate = customer.DeactivationDate
            };
        }
        public static List<DTOCustomer> ToDTO(this List<Model.Customer> customers) {
            var dtoCustomers = new List<DTOCustomer>();
            foreach (var customer in customers) {
                dtoCustomers.Add(customer.ToDTO());
            }
            return dtoCustomers;
        }

        internal static Model.Customer NewToModel(this DTOCustomer dtoCustomer) {
            return new Model.Customer {
                Id = 0, // Id is set to 0 for new records; database will assign the actual Id
                CustomerId = dtoCustomer.CustomerId,
                Name = dtoCustomer.Name,
                Address = dtoCustomer.Address,
                Email = dtoCustomer.Email,
                PhoneNumber = dtoCustomer.PhoneNumber,
                VATNumber = dtoCustomer.VATNumber,
                DeactivationDate = dtoCustomer.DeactivationDate
            };
        }

        internal static Model.Customer DTOToModel(this DTOCustomer dtoCustomer, Model.Customer customer) {
            customer.CustomerId = dtoCustomer.CustomerId;
            customer.Name = dtoCustomer.Name;
            customer.Address = dtoCustomer.Address;
            customer.Email = dtoCustomer.Email;
            customer.PhoneNumber = dtoCustomer.PhoneNumber;
            customer.VATNumber = dtoCustomer.VATNumber;
            customer.DeactivationDate = dtoCustomer.DeactivationDate;

            return customer;
        }
    }
}