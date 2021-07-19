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
            [FromQuery] int chatId,
            [FromQuery] string? search,
            [FromQuery] [Range(1, int.MaxValue)] int page = 1,
            [FromQuery] int items = 5
        ) =>
            (await _messageService.GetMessagesInChatPageAsync(User.GetUserId(), chatId, search, page, items))
            .ToActionResult();

        [HttpPut()]
        public async Task<IActionResult> EditMessage(
            EditMessageDto editMessageDto
        ) =>
            (await _messageService.EditMessageAsync(User.GetUserId(), editMessageDto)).ToActionResult();

        [HttpDelete("{messageId:int}")]
        public async Task<IActionResult> DeleteMessage(
            [FromQuery] int messageId
        ) =>
            (await _messageService.DeleteMessageAsync(User.GetUserId(), messageId)).ToActionResult();
    }
}