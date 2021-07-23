// ReSharper disable All
namespace MessengerApp.Core.DTO.Chat
{
    public class ChatDto
    {
        public ChatDto(int id, string name, int adminId)
        {
            Id = id;
            Name = name;
            AdminId = adminId;
        }

        public int Id { get; }
        
        public string Name { get; }

        public int AdminId { get; }
    }
}