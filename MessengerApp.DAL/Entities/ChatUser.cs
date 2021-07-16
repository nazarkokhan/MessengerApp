using MessengerApp.DAL.Entities.Abstract;
using MessengerApp.DAL.Entities.Authorization;
// ReSharper disable All
#pragma warning disable 8618

namespace MessengerApp.DAL.Entities
{
    public class ChatUser : EntityBase
    {
        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}