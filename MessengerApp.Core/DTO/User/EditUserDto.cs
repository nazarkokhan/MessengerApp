// ReSharper disable All
namespace MessengerApp.Core.DTO.User
{
    public class EditUserDto
    {
        public EditUserDto(int id, string newUserName, string newEmail, string newPassword, string about)
        {
            Id = id;
            NewUserName = newUserName;
            NewEmail = newEmail;
            NewPassword = newPassword;
            About = about;
        }
        public int Id { get; }

        public string NewUserName { get; }
        
        public string NewEmail { get; }

        public string NewPassword { get; }

        public string About { get; set; }
    }
}