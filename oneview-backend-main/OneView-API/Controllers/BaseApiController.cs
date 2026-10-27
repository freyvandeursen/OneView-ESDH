using Microsoft.AspNetCore.Mvc;
using OneView_API.Models;

namespace OneView_API.Controllers {
    [ApiController]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
        protected ObjectResult Problem(string title, string detail, int statusCode)
        {
            var problem = new CustomProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = statusCode
            };

            return StatusCode(statusCode, problem);
        }

        protected ObjectResult Problem(int statusCode, object value)
        {
            return StatusCode(statusCode, value);
        }
    }
}