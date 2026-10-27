using DataAccess.Interface;
using DTO.Customer;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;

namespace Test_Unit {

    [TestFixture]
    internal class DAL_ContactRepository {
        private ICustomerRepository customerRepository;
        private IContactRepository contactRepository;

        [SetUp]
        public async Task SetUp() {
            var serviceProvider = TestDependencySetup.BuildServiceProvider();
            customerRepository = serviceProvider.GetRequiredService<ICustomerRepository>();
            contactRepository = serviceProvider.GetRequiredService<IContactRepository>();
            await TestHelpers.ResetDatabaseWithChangeSql(serviceProvider);
        }

        [Test]
        public void AddContactToCustomer_ShouldAddContact() {
            var customer = new DTOCustomer {
                CustomerId = "CUST102",
                Name = "Bob Johnson",
                Address = "456 Oak Ave",
                Email = "test2@test.com",
                PhoneNumber = "555-5678",
                VATNumber = "VAT67890",
                DeactivationDate = null
            };
            customerRepository.AddCustomer(customer);
            contactRepository.AddContactToCustomer(new DTOContactCreate {
                CustomerId = customer.CustomerId,
                Name = "Contact Name",
                Email = "managerman@realmanager.com",
                Title = "Manager",
                PhoneNumber = "99999999"
            });

            var contacts = contactRepository.GetContactsByCustomerId("CUST102");
            contacts.Should().NotBeNull();
            contacts.Should().HaveCount(1);
            contacts[0].Name.Should().Be("Contact Name");
            contacts[0].Email.Should().Be("managerman@realmanager.com");
            contacts[0].PhoneNumber.Should().Be("99999999");
            contacts[0].Title.Should().Be("Manager");
        }

        [Test]
        public void DeleteContactForCustomer_ShouldRemoveContact() {
            var customer = new DTOCustomer {
                CustomerId = "CUST102",
                Name = "Bob Johnson",
                Address = "456 Oak Ave",
                Email = "test2@test.com",
                PhoneNumber = "555-5678",
                VATNumber = "VAT67890",
                DeactivationDate = null
            };
            customerRepository.AddCustomer(customer);
            contactRepository.AddContactToCustomer(new DTOContactCreate {
                CustomerId = customer.CustomerId,
                Name = "Contact Name",
                Email = "managerman@realmanager.com",
                Title = "Manager",
                PhoneNumber = "99999999"
            });
            var contactsBeforeDeletion = contactRepository.GetContactsByCustomerId("CUST102");
            contactsBeforeDeletion.Should().HaveCount(1);
            var contactID = contactsBeforeDeletion[0].Id;
            contactRepository.DeleteContactForCustomer(contactID);
            contactRepository.GetContactsByCustomerId("CUST102").Should().BeEmpty();

        }
        
    }
}
