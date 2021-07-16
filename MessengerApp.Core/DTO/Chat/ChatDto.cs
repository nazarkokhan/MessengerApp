namespace MessengerApp.Core.DTO.Chat
{
    public class ChatDto
    {
        public ChatDto(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}