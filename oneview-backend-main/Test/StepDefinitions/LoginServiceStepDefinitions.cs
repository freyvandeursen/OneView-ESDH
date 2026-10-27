using BusinessLogic.Interface;
using DataAccess.Interface;
using DTO.Authentication;
using DTO.User;
using FluentAssertions;

namespace Test.StepDefinitions
{
    [Binding]
    public class LoginServiceStepDefinitions
    {
        private readonly ILoginService _loginService;
        private DTOLoginResponse loginResponse;
        private readonly IUserRepository _userRepository;

        public LoginServiceStepDefinitions(ILoginService? loginService, IUserRepository userRepository)
        {
            loginResponse = null!;
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [Given("I log in with a valid username {string} and password {string}")]
        public void GivenILogInWithAValidUsernameAndPassword(string user, string pass)
        {
            var testUser = new DTOUser {
                UserName = user,
                Password = pass,
                Role = "ADMIN"
            };

            _userRepository.AddUser(testUser);

            loginResponse = _loginService.Authenticate(new DTOLoginRequest {
                Username = user,
                Password = pass
            });
        }

        [Then("I should receive a valid authentication token")]
        public void ThenIShouldReceiveAValidAuthenticationToken()
        {
            loginResponse.Should().NotBeNull();
            loginResponse.Token.Should().NotBeNullOrEmpty();
            loginResponse.Token.Length.Should().BeGreaterThan(10);

        }

        [Given("I log in with an invalid username {string} and password {string}")]
        public void GivenILogInWithAnInvalidUsernameAndPassword(string nonuser, string nopass)
        {
            loginResponse = _loginService.Authenticate(new DTOLoginRequest {
                Username = nonuser,
                Password = nopass
            });
        }

        [Then("I should not receive an authentication token")]
        public void ThenIShouldNotReceiveAnAuthenticationToken()
        {
            loginResponse.Should().BeNull();
        }
    }
}
