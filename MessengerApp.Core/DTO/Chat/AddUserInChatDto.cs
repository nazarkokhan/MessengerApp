// ReSharper disable All
namespace MessengerApp.Core.DTO.Chat
{
    public class AddUserInChatDto
    {
        public AddUserInChatDto(int chatId, int userId)
        {
            ChatId = chatId;
            UserId = userId;
        }

        public int ChatId { get; }

        public int UserId { get; }
    }
}