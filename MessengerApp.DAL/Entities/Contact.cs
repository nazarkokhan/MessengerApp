using MessengerApp.DAL.Entities.Abstract;
using MessengerApp.DAL.Entities.Authorization;
// ReSharper disable All
#pragma warning disable 8618

namespace MessengerApp.DAL.Entities
{
    public class Contact : EntityBase
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int UserContactId { get; set; }
        public User UserContact { get; set; }
    }
}