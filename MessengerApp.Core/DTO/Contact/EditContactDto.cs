// ReSharper disable All
namespace MessengerApp.Core.DTO.Contact
{
    public class EditContactDto
    {
        public EditContactDto(int userContactId, int newUserContactId)
        {
            UserContactId = userContactId;
            NewUserContactId = newUserContactId;
        }

        public int UserContactId { get; }
        
        public int NewUserContactId { get; }
    }
}