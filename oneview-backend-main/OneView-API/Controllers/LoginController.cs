using BusinessLogic.Interface;
using OneView_API.Models;
using Swashbuckle.AspNetCore.Annotations;
using DTO.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace OneView_API.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class LoginController : BaseApiController {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService) {
            _loginService = loginService;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Login user",
            Description = "Authenticating the user and responds whether it was successful or not."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Login successful", typeof(DTOLoginResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid credentials", typeof(CustomProblemDetails))]
        public IActionResult Login([FromBody] DTOLoginRequest request) {
            var response = _loginService.Authenticate(request);
            return response == null ? Problem("Invalid Credentials", "The typed credentials are invalid",
                    StatusCodes.Status400BadRequest) : Ok(response);
        }
    }
}
