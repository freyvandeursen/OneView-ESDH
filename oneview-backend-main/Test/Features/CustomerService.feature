Feature: CustomerService

Feature concerned with crud operations on customer objects

@tag1
Scenario: Add a new customer
	Given I create a new customer with ID "CUST777"
	Then the customer should be retrievable by ID "CUST777"

@tag2
Scenario: Deactivate a customer
	Given I create a new customer with ID "CUST888"
	When I deactivate the customer with ID "CUST888"
	Then the customer with ID "CUST888" should be marked as inactive

@tag3
Scenario: Get all customers 
	Given I create a new customer with ID "CUST999"
	When I retrieve all customers
	Then the list of customers should include the customer with ID "CUST999"


@tag4
Scenario: Create 10 new customers and verify they exists
	Given I create a new customer with ID "<CustomerId>"
	When I retrieve all customers
	Then the list of customers should include the customer with ID "<CustomerId>"
	Examples: 
		| CustomerId |
		| CUST001    |
		| CUST002    |
		| CUST003    |
		| CUST004    |
		| CUST005    |
		| CUST006    |
		| CUST007    |
		| CUST008    |
		| CUST009    |
	    | CUST010    |

 @tag5
 Scenario: Attempt to create a customer with a duplicate ID
 Given I create a new customer with ID "CUST0000"
 When I create a new customer with ID "CUST0000"
 Then an error should be raised indicating a duplicate customer ID