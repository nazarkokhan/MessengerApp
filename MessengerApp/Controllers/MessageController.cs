using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO.Message;
using MessengerApp.Extensions;
using Microsoft.AspNetCore.Mvc;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;

namespace MessengerApp.Controllers
{
    [BearerAuthorize(Roles.Admin | Roles.User)]
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("{chatId:int}")]
        public async Task<IActionResult> GetMessagesInChat(
            int chatId,
            [FromQuery] string? search,
            [FromQuery] [Range(1, int.MaxValue)] int page = 1,
            [FromQuery] int items = 5
        ) =>
            (await _messageService.GetMessagesInChatPageAsync(User.GetUserId(), chatId, search, page, items))
            .ToActionResult();

        [HttpPost("{chatId:int}")]
        public async Task<IActionResult> CreateMessage(
            [Range(1, int.MaxValue)] int chatId,
            CreateMessageDto createMessageDto
        ) =>
            (await _messageService.CreateMessageAsync(User.GetUserId(), chatId, createMessageDto))
            .ToActionResult();

        [HttpPut]
        public async Task<IActionResult> EditMessage(
            EditMessageDto editMessageDto
        ) =>
            (await _messageService.EditMessageAsync(User.GetUserId(), editMessageDto))
            .ToActionResult();

        [HttpDelete("{messageId:int}")]
        public async Task<IActionResult> DeleteMessage(
            [Range(1, long.MaxValue)] long messageId
        ) =>
            (await _messageService.DeleteMessageAsync(User.GetUserId(), messageId))
            .ToActionResult();
    }
}