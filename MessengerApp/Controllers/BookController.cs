using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO.Book;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApp.Controllers
{
    [BearerAuthorize(Role.User | Role.Admin)]
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooksAsync(
            [FromQuery] string? search,
            [FromQuery] [Range(1, int.MaxValue)] int page = 1,
            [FromQuery] int items = 5
        )
            => (await _bookService.GetBooksAsync(page, items, search)).ToActionResult();

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBookAsync([Range(0, int.MaxValue)] int id)
            => (await _bookService.GetBookAsync(id)).ToActionResult();

        [HttpPost]
        public async Task<IActionResult> CreateBookAsync(CreateBookDto book)
            => (await _bookService.CreateBookAsync(book)).ToActionResult();

        [HttpPut]
        public async Task<IActionResult> UpdateBookAsync(UpdateBookDto book)
            => (await _bookService.UpdateBookAsync(book)).ToActionResult();

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBookAsync([Range(0, int.MaxValue)] int id)
            => (await _bookService.DeleteBookAsync(id)).ToActionResult();
    }
}