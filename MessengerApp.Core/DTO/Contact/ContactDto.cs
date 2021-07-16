// ReSharper disable All
namespace MessengerApp.Core.DTO.Contact
{
    public class ContactDto
    {
        public ContactDto(string userName, string about, string email)
        {
            UserName = userName;
            About = about;
            Email = email;
        }

        public string UserName { get; }

        public string About { get; }

        public string Email { get; }
    }
}