using MessengerApp.Hubs;
using MessengerApp.Services.Abstraction;
using Microsoft.AspNetCore.SignalR;

namespace MessengerApp.Services
{
    public class HubService : IHubService
    {
        private readonly IHubContext<ChatHub> _chatHub;

        public HubService(IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub;
        }

        // public async Task Do()
        // {
        //     _chatHub.Clients.
        // }
    }
}