// ReSharper disable All
namespace MessengerApp.Core.DTO.Contact
{
    public class ContactDto
    {
        public ContactDto(int id, string userName, string about, string email)
        {
            Id = id;
            UserName = userName;
            About = about;
            Email = email;
        }

        public int Id { get; set; }
        
        public string UserName { get; }

        public string About { get; }

        public string Email { get; }
    }
}