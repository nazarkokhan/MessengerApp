// ReSharper disable All
namespace MessengerApp.Core.DTO.Contact
{
    public class CreateContactDto
    {
        public CreateContactDto(int userContactId)
        {
            UserContactId = userContactId;
        }

        public int UserContactId { get; }
    }
}