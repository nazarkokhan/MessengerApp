namespace MessengerApp.Core.DTO.Chat
{
    public class ChatDto
    {
        public ChatDto(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        
        public string Name { get; }
    }
}