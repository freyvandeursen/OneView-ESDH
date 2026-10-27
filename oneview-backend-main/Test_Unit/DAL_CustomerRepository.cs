using DataAccess.Interface;
using DTO.Customer;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;

namespace Test_Unit {
    [TestFixture]
    public class DAL_CustomerRepository {
        private ICustomerRepository customerRepository;

        [SetUp]
        public async Task SetUp() {
            var serviceProvider = TestDependencySetup.BuildServiceProvider();
            customerRepository = serviceProvider.GetRequiredService<ICustomerRepository>();
            await TestHelpers.ResetDatabaseWithChangeSql(serviceProvider);
        }

        [Test]
        public void AddCustomer_ShouldAddCustomerRetrievableByID() {

            //Arrange
            var customer = new DTOCustomer {
                CustomerId = "CUST777",
                Name = "Janey doe",
                Address = "Summin st."
            };

            //Act
            customerRepository.AddCustomer(customer);
            var retrievedCustomer = customerRepository.GetCustomerById("CUST777");

            //Assert       
            retrievedCustomer.Should().NotBeNull();
            retrievedCustomer.CustomerId.Should().Be("CUST777");
            retrievedCustomer.Name.Should().Be("Janey doe");
            retrievedCustomer.Address.Should().Be("Summin st.");
        }

        [Test]
        public void GetCustomerById_NonExistentCustomer_ShouldReturnNull() {
            //Act
            var retrievedCustomer = customerRepository.GetCustomerById("NONEXISTENT");
            //Assert       
            retrievedCustomer.Should().BeNull();
        }

        [Test]
        public void GetCustomersById_ShouldReturnAspecificCustomer() {
            //Arrange
            var customer1 = new DTOCustomer {
                CustomerId = "CUST101",
                Name = "Alice Smith",
                Address = "123 Main St",
                Email = "test@test.com",
                PhoneNumber = "555-1234",
                VATNumber = "VAT12345",
                DeactivationDate = new DateTime(2025, 12, 31)

            };
            var customer2 = new DTOCustomer {
                CustomerId = "CUST102",
                Name = "Bob Johnson",
                Address = "456 Oak Ave",
                Email = "test2@test.com",
                PhoneNumber = "555-5678",
                VATNumber = "VAT67890",
                DeactivationDate = new DateTime(2026, 11, 30)

            };
            customerRepository.AddCustomer(customer1);
            customerRepository.AddCustomer(customer2);
            //Act
            var retrievedCustomer = customerRepository.GetCustomerById("CUST102");

            //Assert       
            retrievedCustomer.Should().NotBeNull();
            retrievedCustomer.CustomerId.Should().Be("CUST102");
            retrievedCustomer.Name.Should().Be("Bob Johnson");
            retrievedCustomer.Address.Should().Be("456 Oak Ave");
            retrievedCustomer.Email.Should().Be("test2@test.com");
            retrievedCustomer.PhoneNumber.Should().Be("555-5678");
            retrievedCustomer.VATNumber.Should().Be("VAT67890");
            retrievedCustomer.DeactivationDate.Should().Be(new DateTime(2026, 11, 30));
        }

        [Test]
        public void GetCustomers_ShouldReturnAlist() {
            //Arrange
            var customer1 = new DTOCustomer {
                CustomerId = "CUST101",
                Name = "Alice Smith",
                Address = "123 Main St",
                Email = "test@test.com",
                PhoneNumber = "555-1234",
                VATNumber = "VAT12345",
                DeactivationDate = new DateTime(2025, 12, 31)

            };
            var customer2 = new DTOCustomer {
                CustomerId = "CUST102",
                Name = "Bob Johnson",
                Address = "456 Oak Ave",
                Email = "test2@test.com",
                PhoneNumber = "555-5678",
                VATNumber = "VAT67890",
                DeactivationDate = new DateTime(2026, 11, 30)

            };
            customerRepository.AddCustomer(customer1);
            customerRepository.AddCustomer(customer2);
            //Act
            var customers = customerRepository.GetCustomers();
            var selectedCustomers = 
                from c in customers let v = c.CustomerId! where v is "CUST101" or "CUST102" select c;

            //Assert
            selectedCustomers.Should().HaveCount(2);
        }

        [Test]
        public void GetCustomers_ShouldMatchChangeSql() {
            //Act
            var customers = customerRepository.GetCustomers();
            //Assert
            _ = customers!.Count.Should().Be(3);
        }

        [Test]
        public void UpdateCustomer_ShouldModifyExistingCustomer() {
            //Arrange
            var customer = new DTOCustomer {
                CustomerId = "CUST102",
                Name = "Bob Johnson",
                Address = "456 Oak Ave",
                Email = "test2@test.com",
                PhoneNumber = "555-5678",
                VATNumber = "VAT67890",
                DeactivationDate = new DateTime(2026, 11, 30)
            };
            customerRepository.AddCustomer(customer);
            //Act
            customer.CustomerId = "CUST102";
            customer.Name = "Updated Name";
            customer.Address = "Updated Address";
            customer.Email = "test@test.com";
            customer.PhoneNumber = "555-0000";
            customer.VATNumber = "VAT00000";
            customer.DeactivationDate = new DateTime(2027, 1, 1);
            var updateResult = customerRepository.UpdateCustomer(customer);
            var updatedCustomer = customerRepository.GetCustomerById("CUST102");
            //Assert       
            updateResult.Should().BeTrue();
            updatedCustomer.Should().NotBeNull();
            updatedCustomer.Name.Should().Be("Updated Name");
            updatedCustomer.Address.Should().Be("Updated Address");
            updatedCustomer.Email.Should().Be("test@test.com");
            updatedCustomer.PhoneNumber.Should().Be("555-0000");
            updatedCustomer.VATNumber.Should().Be("VAT00000");
            updatedCustomer.DeactivationDate.Should().Be(new DateTime(2027, 1, 1));
        }
    }
}
