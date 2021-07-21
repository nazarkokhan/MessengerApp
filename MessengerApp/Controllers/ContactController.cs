using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO.Contact;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApp.Controllers
{
    [BearerAuthorize(Roles.Admin | Roles.User)]
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<IActionResult> GetContactsPage(
            [FromQuery] string? search,
            [FromQuery] [Range(1, int.MaxValue)] int page = 1,
            [FromQuery] int items = 5
        ) =>
            (await _contactService.GetContactsPageAsync(User.GetUserId(), search, page, items)).ToActionResult();

        [HttpGet("{userContactId:int}")]
        public async Task<IActionResult> GetContact(
            int userContactId
        ) =>
            (await _contactService.GetContactAsync(User.GetUserId(), userContactId)).ToActionResult();

        [HttpPost]
        public async Task<IActionResult> CreateContact(
            CreateContactDto createContactDto
        ) =>
            (await _contactService.CreateContactAsync(User.GetUserId(), createContactDto)).ToActionResult();

        [HttpPut]
        public async Task<IActionResult> EditContact(
            EditContactDto editContactDto
        ) =>
            (await _contactService.EditContactAsync(User.GetUserId(), editContactDto)).ToActionResult();

        [HttpDelete("{contactId:int}")]
        public async Task<IActionResult> DeleteContact(
            int contactId
        ) =>
            (await _contactService.DeleteContactAsync(User.GetUserId(), contactId)).ToActionResult();
    }
}