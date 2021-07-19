// ReSharper disable All
namespace MessengerApp.Core.DTO.Chat
{
    public class EditChatDto
    {
        public EditChatDto(int id, string newName, int adminId)
        {
            Id = id;
            NewName = newName;
            AdminId = adminId;
        }
        public int Id { get; set; }

        public string NewName { get; }

        public int AdminId { get; set; }
    }
}