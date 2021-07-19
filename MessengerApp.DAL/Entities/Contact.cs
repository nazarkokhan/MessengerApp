using System.ComponentModel.DataAnnotations.Schema;
using MessengerApp.Core.DTO.Contact;
using MessengerApp.DAL.Entities.Abstract;
using MessengerApp.DAL.Entities.Authorization;
// ReSharper disable All
#pragma warning disable 8618

namespace MessengerApp.DAL.Entities
{
    public class Contact : EntityBase
    {
        // [ForeignKey("User")]
        public int UserId { get; set; }
        // [ForeignKey("UserId")]
        public User User { get; set; }

        // [ForeignKey("UserContact")]
        public int? UserContactId { get; set; }
        // [ForeignKey("UserContactId")]
        public User UserContact { get; set; }

        public void MapEditContact(EditContactDto editContactDto)
        {
            UserContactId = editContactDto.NewUserContactId;
        }
    }
}