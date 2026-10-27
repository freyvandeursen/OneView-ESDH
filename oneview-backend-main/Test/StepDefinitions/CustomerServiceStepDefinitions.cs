using BusinessLogic.Interface;
using DTO.Customer;
using FluentAssertions;

namespace Test.StepDefinitions {
    [Binding]
    public class CustomerServiceStepDefinitions {
        private readonly ICustomerService _customerService;

        private DTOCustomer? newCustomer;
        private Exception? caughtException;

        public CustomerServiceStepDefinitions(ICustomerService customerService) {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }


        [Given("I create a new customer with ID {string}")]
        [When("I create a new customer with ID {string}")]
        public void GivenICreateANewCustomerWithID(string customerId) {
            newCustomer = new DTOCustomer {
                CustomerId = customerId,
                Name = "Johnny Doey",
                Address = "Electric ave."
            };
            try {
                _customerService.AddCustomer(newCustomer);
            }
            catch (Exception ex) {
                ex.Should().BeOfType<ArgumentException>();
                caughtException = ex;
            }
        }


        [Then("the customer should be retrievable by ID {string}")]
        public void ThenTheCustomerShouldBeRetrievableByID(string customerId) {
            var retrievedCustomer = _customerService.GetCustomerById(customerId);
            retrievedCustomer.Should().BeEquivalentTo(newCustomer);
        }

        [When("I deactivate the customer with ID {string}")]
        public void WhenIDeactivateTheCustomerWithID(string customerID) {
            _customerService.DeactivateCustomer(customerID, DateTime.Now);
        }

        [Then("the customer with ID {string} should be marked as inactive")]
        public void ThenTheCustomerWithIDShouldBeMarkedAsInactive(string customerID) {
            var retrievedCustomer = _customerService.GetCustomerById(customerID);
            retrievedCustomer!.DeactivationDate.Should().NotBeNull();
        }

        [When("I retrieve all customers")]
        public void WhenIRetrieveAllCustomers() {
            _customerService.GetCustomers();
        }

        [Then("the list of customers should include the customer with ID {string}")]
        public void ThenTheListOfCustomersShouldIncludeTheCustomerWithID(string customerID) {
            var customers = _customerService.GetCustomers();
            customers.Should().Contain(c => c.CustomerId == customerID);
        }

        [Then("an error should be raised indicating a duplicate customer ID")]
        public void ThenAnErrorShouldBeRaisedIndicatingADuplicateCustomerID() {
            caughtException.Should().NotBeNull();
            caughtException.Message.Should().Contain("already exists");
        }




    }
}
