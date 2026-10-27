using BusinessLogic.Interface;
using DTO.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneView_API.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace OneView_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContextController : BaseApiController
    {
        private readonly IContextService _contextService;

        public ContextController(IContextService contextService)
        {
            _contextService = contextService;
        }

        [HttpGet("{customerId}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Gets all Context items related to the CustomerId",
            Description = "Returns all context item related to the customer with CustomerId."
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Context item(s) found",
            typeof(List<DTOContext>)
        )]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not authorized")]
        [SwaggerResponse(
            StatusCodes.Status404NotFound,
            "Customer not found",
            typeof(CustomProblemDetails)
        )]
        [SwaggerResponse(
            StatusCodes.Status500InternalServerError,
            "Internal server error",
            typeof(CustomProblemDetails)
        )]
        public async Task<ActionResult<List<DTOContext>>> GetAllContextByCustomerId(
            string customerId
        )
        {
            try
            {
                var contextList = await _contextService.GetAllContextByCustomerIdAsync(customerId);

                return contextList.Any()
                    ? Ok(contextList)
                    : Problem(
                        "No Context Found",
                        $"No contexts found for customer ID {customerId}.",
                        StatusCodes.Status404NotFound
                    );
            }
            catch (Exception ex)
            {
                return Problem(
                    "Internal Server Error",
                    ex.Message,
                    StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}