using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("chats")]
        public async Task<IActionResult> GetChatPage(
            [FromQuery] string? search,
            [FromQuery] [Range(1, int.MaxValue)] int page = 1,
            [FromQuery] int items = 5
        ) =>
            (await _chatService.GetChatsPageAsync(search, page, items)).ToActionResult();

        [HttpGet("my-chats")]
        public async Task<IActionResult> GetUserChatsPage(
            [FromQuery] string? search,
            [FromQuery] [Range(1, int.MaxValue)] int page = 1,
            [FromQuery] int items = 5
        ) =>
            (await _chatService.GetUserChatsPageAsync(default, search, page, items)).ToActionResult();

        [HttpGet("my-chats")]
        public async Task<IActionResult> GetChat(
        ) =>
            (await _chatService.GetChatAsync(default)).ToActionResult();
    }
}