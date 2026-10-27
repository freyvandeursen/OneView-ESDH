Feature: LoginService

Feature concerned with authenticating login attempts against the user repository

@tag1
Scenario: Logging in with an existing user should return a token
	Given I log in with a valid username "user" and password "pass"
	Then I should receive a valid authentication token

@tag2
Scenario: Logging in with a non-existing user should not return a token
	Given I log in with an invalid username "nonuser" and password "nopass"
	Then I should not receive an authentication token

