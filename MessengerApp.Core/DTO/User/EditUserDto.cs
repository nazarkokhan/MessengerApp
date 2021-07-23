// ReSharper disable All
namespace MessengerApp.Core.DTO.User
{
    public class EditUserDto
    {
        public EditUserDto(string userName, string about)
        {
            UserName = userName;
            About = about;
        }

        public string UserName { get; }

        public string About { get; }
    }
}