using System.Threading.Tasks;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO.Message;
using MessengerApp.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace MessengerApp.Hubs
{
    [BearerAuthorize]
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task SendMessage(string message)
        {
            var userId = Context.User!.GetUserId();

            var messageDto = await _messageService.CreateMessageAsync(userId, 1, new CreateMessageDto(message));

            await Clients.All.SendAsync("Send", messageDto.Data);
        }
    }
}