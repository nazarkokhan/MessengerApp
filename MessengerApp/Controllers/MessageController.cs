using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("contacts")]
        public async Task<IActionResult> GetMessagesInChat(
            [FromQuery] string? search,
            [FromQuery] [Range(1, int.MaxValue)] int page = 1,
            [FromQuery] int items = 5
        ) =>
            (await _messageService.GetMessagesInChatPageAsync(default, search, page, items)).ToActionResult();
    }
}