// ReSharper disable All
namespace MessengerApp.Core.DTO.Chat
{
    public class CreateChatDto
    {
        public CreateChatDto(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}