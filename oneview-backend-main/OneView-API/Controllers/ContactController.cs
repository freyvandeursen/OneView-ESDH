using BusinessLogic.Interface;
using DTO.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OneView_API.Controllers {
    [ApiController]
    [Route("customers/{customerId}/contacts")]
    public class ContactController : BaseApiController {

        private readonly IContactService contactService;

        public ContactController(IContactService contactService) {
            this.contactService = contactService;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<List<DTOContact>> GetContacts(string customerId) {
            var contacts = contactService.GetContactsByCustomerId(customerId);
            return Ok(contacts);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddContact([FromRoute] string customerId, [FromBody] DTOContactCreate contact) {
            Console.WriteLine($"Controller HIT - customerId={customerId}");
            Console.WriteLine($"Contact body: {System.Text.Json.JsonSerializer.Serialize(contact)}");
            contact.CustomerId = customerId;
            contactService.AddContactToCustomer(contact);
            return CreatedAtAction(nameof(GetContacts), new { customerId }, contact);
        }

        [HttpDelete("{contactId}")]
        [Authorize]
        public IActionResult DeleteContact([FromRoute] string customerId, [FromRoute] int contactId) {
            try {
                contactService.DeleteContactForCustomer(contactId);
                return NoContent();
            }
            catch (ArgumentException ex) {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
