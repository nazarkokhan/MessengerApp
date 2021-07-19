using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO.Chat;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApp.Controllers
{
    [BearerAuthorize(Roles.Admin | Roles.User)]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        public async Task<IActionResult> GetChatPage(
            [FromQuery] string? search,
            [FromQuery] [Range(1, int.MaxValue)] int page = 1,
            [FromQuery] int items = 5
        ) =>
            (await _chatService.GetChatsPageAsync(search, page, items)).ToActionResult();

        [HttpGet("my")]
        public async Task<IActionResult> GetUserChatsPage(
            [FromQuery] string? search,
            [FromQuery] [Range(1, int.MaxValue)] int page = 1,
            [FromQuery] int items = 5
        ) =>
            (await _chatService.GetUserChatsPageAsync(User.GetUserId(), search, page, items)).ToActionResult();

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetChat(
            [FromQuery] int id
        ) =>
            (await _chatService.GetChatAsync(id)).ToActionResult();

        [HttpPost]
        public async Task<IActionResult> CreateChat(
            CreateChatDto createChatDto
        ) =>
            (await _chatService.CreateChatAsync(User.GetUserId(), createChatDto)).ToActionResult();

        [HttpPut]
        public async Task<IActionResult> EditChat(
            EditChatDto editChatDto
        ) =>
            (await _chatService.EditChatAsync(User.GetUserId(), editChatDto)).ToActionResult();

        [HttpDelete]
        public async Task<IActionResult> DeleteChat(
            [FromQuery] [Range(1, int.MaxValue)] int id
        ) =>
            (await _chatService.DeleteChatAsync(User.GetUserId(), id)).ToActionResult();
    }
}