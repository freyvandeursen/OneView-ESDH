using BusinessLogic.Interface;
using DTO.Label;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace OneView_API.Controllers {
    /// <summary>
    /// Manages labels and their relationships with customers and context items.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class LabelController : BaseApiController {
        private readonly ILabelService _labelService;
        private readonly ILogger<LabelController> _logger;

        public LabelController(ILabelService labelService, ILogger<LabelController> logger) {
            _labelService = labelService;
            _logger = logger;
        }

        // Vi samler al logging i en simpel helper for mere laesbar kode
        private IActionResult ServerError(Exception ex, string actionName) {
            _logger.LogError(ex, "[ERROR] {ActionName}: {Exception}", actionName, ex);
            return Problem(
                "Internal Server Error",
                ex.Message,
                StatusCodes.Status500InternalServerError
            );
        }

        [HttpGet("customer/{customerId}")]
        [Authorize]
        [SwaggerOperation(Summary = "Gets all labels related to a customer")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(List<DTOLabel>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid customerId")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No labels found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error")]
        public async Task<IActionResult> GetAllLabelsByCustomerId(string customerId) {
            if (string.IsNullOrWhiteSpace(customerId)) {
                return BadRequest("customerId is required.");
            }

            try {
                var labels = await _labelService.GetAllLabelsByCustomerIdAsync(customerId);

                return labels?.Any() != true
                    ? Problem(
                        "No Labels Found",
                        $"No labels exist for customer ID '{customerId}'.",
                        StatusCodes.Status404NotFound
                    ): Ok(labels);
            }
            catch (Exception ex)
            {
                return ServerError(ex, nameof(GetAllLabelsByCustomerId));
            }
}

        [HttpGet("context/{contextId}")]
        [Authorize]
        [SwaggerOperation(Summary = "Gets all labels applied to a context item")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(List<DTOLabel>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid contextId")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No labels found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error")]
        public async Task<IActionResult> GetAllLabelsByContextId(int contextId)
        {
            try
            {
                var labels = await _labelService.GetAllLabelsByContextIdAsync(contextId);

                return labels?.Any() != true
                    ? Problem(
                        "No Labels Found",
                        $"No labels are applied to context ID '{contextId}'.",
                        StatusCodes.Status404NotFound
                    )
                    : Ok(labels);
            }
            catch (Exception ex)
            {
                return ServerError(ex, nameof(GetAllLabelsByContextId));
            }
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Creates a new label")]
        public async Task<IActionResult> CreateNewLabel([FromBody] DTOLabelNew labelDtoNew)
        {
            if (labelDtoNew == null || string.IsNullOrWhiteSpace(labelDtoNew.Name)) 
            {
                return Problem(
                    "Invalid Input",
                    "Name is required.",
                    StatusCodes.Status400BadRequest
                );
            }

            try
            {   
                var labelDto = new DTOLabel
                {
                    CustomerId = labelDtoNew.CustomerId,
                    ContextId = labelDtoNew.ContextId,
                    Name = labelDtoNew.Name,
                    Description = labelDtoNew.Description,
                    Value = labelDtoNew.Value
                };

                var created = await _labelService.CreateNewLabelAsync(labelDto);

                return created
                    ? Ok(new { Message = $"Label '{labelDto.Name}' created successfully." })
                    : Problem(
                        "Creation Failed",
                        "Could not create label.",
                        StatusCodes.Status400BadRequest
                    );
            }
            catch (Exception ex)
            {
                return ServerError(ex, nameof(CreateNewLabel));
            }
        }

        [HttpPut]
        [Authorize]
        [SwaggerOperation(
            Summary = "Updates any field on an existing label",
            Description = "Use this method to add a label to a context"
        )]
        public async Task<IActionResult> UpdateLabelData([FromBody] DTOLabel labelDto)
        {
            try
            {
                var updated = await _labelService.UpdateLabelAsync(labelDto);
                return updated ? Ok(true) : Problem(
                    "Label Not Found",
                    "The label to update could not be found or updated.",
                    StatusCodes.Status404NotFound
                );
            }
            catch (Exception ex)
            {
                return ServerError(ex, nameof(UpdateLabelData));
            }
        }

        [HttpDelete]
        [SwaggerOperation(Summary = "Deletes a label and all its relations")]
        public async Task<IActionResult> DeleteLabel([FromBody] DTOLabel labelDto)
        {
            if (labelDto == null) 
            { 
                return Problem(
                    "Invalid Payload",
                    "Label DTO required.",
                    StatusCodes.Status400BadRequest
                );        
            }

            try
            {
                var deleted = await _labelService.DeleteLabelAsync(labelDto);
                return deleted ? Ok() : Problem(
                    "Label Not Found",
                    "The label could not be deleted because it does not exist or has already been removed.",
                    StatusCodes.Status404NotFound
                );
            }
            catch (Exception ex)
            {
                return ServerError(ex, nameof(DeleteLabel));
            }
        }
    }
}
