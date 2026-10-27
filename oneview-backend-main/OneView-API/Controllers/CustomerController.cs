using BusinessLogic.Interface;
using DTO.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneView_API.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace OneView_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : BaseApiController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{CustomerId}")]
        [Authorize] // <-- Brugeren skal være authenticated (rolle er underordnet)
        [SwaggerOperation(
            Summary = "Gets a customer by ID",
            Description = "Returns customer details if the specified customer exists in the database."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Customer found", typeof(DTOCustomer))]
        // VVV -- Denne virker først når [Authorize] er implementeret -- VVV
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
        public ActionResult<DTOCustomer> GetCustomerById(string CustomerId)
        {
            try
            {
                var customer = _customerService.GetCustomerById(CustomerId);
                return customer == null
                    ? Problem(
                        "Customer Not Found",
                        $"No customer found with ID {CustomerId}.",
                        StatusCodes.Status404NotFound
                    )
                    : Ok(customer);
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

        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Gets all customers",
            Description = "Returns a list of all customers in the database. If no customer exist, a 404 response is returned."
        )]
        [SwaggerResponse(
            StatusCodes.Status200OK,
            "Customers retrieved successfully",
            typeof(List<DTOCustomer>)
        )]
        [SwaggerResponse(
            StatusCodes.Status404NotFound,
            "No customers found",
            typeof(CustomProblemDetails)
        )]
        [SwaggerResponse(
            StatusCodes.Status500InternalServerError,
            "Internal server error",
            typeof(CustomProblemDetails)
        )]
        public ActionResult<List<DTOCustomer>> GetCustomers()
        {
            try
            {
                var customers = _customerService.GetCustomers();
                return customers == null
                    ? Problem(
                        "No Customers Found",
                        "There are no customers in the database.",
                        StatusCodes.Status404NotFound
                    )
                    : Ok(customers);
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

        [HttpPost]
        [Authorize(Roles = "ADMIN")] // <-- Brugeren skal være authenticated som ADMIN
        [SwaggerOperation(
            Summary = "Adds a new customer (ADMIN)",
            Description = "Creates a new customer in the database. Returns 201 Created with the customer data if successful."
        )]
        [SwaggerResponse(
            StatusCodes.Status201Created,
            "Customer created successfully",
            typeof(DTOCustomer)
        )]
        [SwaggerResponse(
            StatusCodes.Status400BadRequest,
            "Invalid customer data or customer already exists",
            typeof(CustomProblemDetails)
        )]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not authorized")]
        [SwaggerResponse(
            StatusCodes.Status500InternalServerError,
            "Internal server error",
            typeof(CustomProblemDetails)
        )]
        public IActionResult AddCustomer([FromBody] DTOCustomer dtoCustomer)
        {
            try
            {
                if (dtoCustomer == null)
                {
                    return Problem(
                        "Invalid Customer Data",
                        "The customer data provided is null.",
                        StatusCodes.Status400BadRequest
                    );
                }

                _customerService.AddCustomer(dtoCustomer);
                return CreatedAtAction(
                    nameof(GetCustomerById),
                    new { dtoCustomer.CustomerId },
                    dtoCustomer
                );
            }
            catch (ArgumentException argEx)
            {
                return Problem(
                    "Customer Already Exists",
                    argEx.Message,
                    StatusCodes.Status400BadRequest
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

        [HttpDelete("{CustomerId}/{DeactivationDate:datetime}")]
        [Authorize(Roles = "ADMIN")]
        [SwaggerOperation(
            Summary = "Deactivate a customer (ADMIN)",
            Description = "Sets the deactivation date for the specified customer."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Customer deactivated successfully")]
        [SwaggerResponse(
            StatusCodes.Status400BadRequest,
            "Invalid customer ID or date",
            typeof(CustomProblemDetails)
        )]
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
        public IActionResult DeactivateCustomer(string CustomerId, DateTime DeactivationDate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CustomerId))
                {
                    return Problem(
                        "Invalid Customer ID",
                        "The provided customer ID is null or empty.",
                        StatusCodes.Status400BadRequest
                    );
                }

                _customerService.DeactivateCustomer(CustomerId, DeactivationDate);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return Problem(
                    "Customer Not Found",
                    $"No customer found with ID {CustomerId}.",
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

        [HttpPut("{CustomerId}")]
        [Authorize(Roles = "ADMIN")]
        [SwaggerOperation(
            Summary = "Activates a customer",
            Description = "Reactivates a previously deactivated customer by clearing the deactivation date."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Customer activated successfully")]
        [SwaggerResponse(
            StatusCodes.Status400BadRequest,
            "Invalid customer ID",
            typeof(CustomProblemDetails)
        )]
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
        public IActionResult ActivateCustomer(string CustomerId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CustomerId))
                {
                    return Problem(
                        "Invalid Customer ID",
                        "The provided customer ID is null or empty.",
                        StatusCodes.Status400BadRequest
                    );
                }

                _customerService.DeactivateCustomer(CustomerId, null);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return Problem(
                    "Customer Not Found",
                    $"No customer found with ID {CustomerId}.",
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
