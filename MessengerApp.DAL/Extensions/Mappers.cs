using System.Collections.Generic;
using MessengerApp.Core.DTO.Chat;
using MessengerApp.Core.DTO.Contact;
using MessengerApp.DAL.Entities;

namespace MessengerApp.DAL.Extensions
{
    public static class Mappers
    {
        public static Chat MapChat(this CreateChatDto createChatDto, int userId)
        {
            return new()
            {
                Name = createChatDto.Name,
                AdminId = userId,
                ChatUsers = new List<ChatUser>
                {
                    new() { UserId = userId }
                }
            };
        }

        public static Contact MapContact(this CreateContactDto createContactDto, int userId)
        {
            return new()
            {
                UserId = userId,
                UserContactId = createContactDto.UserContactId
            };
        }

        public static Contact MapContact(this EditContactDto editContactDto, int userId)
        {
            return new()
            {
                UserId = userId,
                UserContactId = editContactDto.UserContactId
            };
        }
    }
}