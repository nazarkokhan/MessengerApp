// ReSharper disable All
namespace MessengerApp.Core.DTO.Chat
{
    public class EditChatDto
    {
        public EditChatDto(string newName)
        {
            NewName = newName;
        }

        public string NewName { get; }

        public int AdminId { get; set; }
    }
}