using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO.Author;
using MessengerApp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApp.Controllers
{
    // [BearerAuthorize(Role.User | Role.Admin)]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthorsAsync(
            [FromQuery] string? search,
            [FromQuery] [Range(1, int.MaxValue)] int page = 1,
            [FromQuery] int items = 5
        )
            => (await _authorService.GetAuthorsAsync(page, items, search)).ToActionResult();

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAuthorAsync([Range(0, int.MaxValue)] int id)
            => (await _authorService.GetAuthorAsync(id)).ToActionResult();

        [HttpPost]
        public async Task<IActionResult> CreateAuthorAsync(CreateAuthorDto author)
            => (await _authorService.CreateAuthorAsync(author)).ToActionResult();

        [HttpPut]
        public async Task<IActionResult> UpdateAuthorAsync(UpdateAuthorDto author)
            => (await _authorService.UpdateAuthorAsync(author)).ToActionResult();

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAuthorAsync([Range(0, int.MaxValue)] int id)
            => (await _authorService.DeleteAuthorAsync(id)).ToActionResult();
    }
}