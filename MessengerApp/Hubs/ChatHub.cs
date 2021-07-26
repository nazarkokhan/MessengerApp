using System.Linq;
using System.Threading.Tasks;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Extensions;
using Microsoft.AspNetCore.Mvc;
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
            
            await Clients.All.SendAsync("Send", userId, message);
            
            // _messageService.CreateMessageAsync(userId)
        }
    }
}