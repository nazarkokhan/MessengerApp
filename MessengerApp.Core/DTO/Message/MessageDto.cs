using System;
// ReSharper disable All

namespace MessengerApp.Core.DTO.Message
{
    public class MessageDto
    {
        public MessageDto(long id, string body, DateTime dateTime, int userId, int chatId)
        {
            Id = id;
            Body = body;
            DateTime = dateTime;
            UserId = userId;
            ChatId = chatId;
        }

        public long Id { get; set; }
        
        public string Body { get; }

        public DateTime DateTime { get; }

        public int UserId { get; }

        public int ChatId { get; }
    }
}