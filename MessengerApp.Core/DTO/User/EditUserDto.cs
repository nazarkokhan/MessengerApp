// ReSharper disable All
namespace MessengerApp.Core.DTO.User
{
    public class EditUserDto
    {
        public EditUserDto(string newUserName, string newEmail, string newPassword, string about, int id)
        {
            NewUserName = newUserName;
            NewEmail = newEmail;
            NewPassword = newPassword;
            About = about;
            Id = id;
        }

        public string NewUserName { get; }
        
        public string NewEmail { get; }

        public string NewPassword { get; }

        public string About { get; set; }

        public int Id { get; }
    }
}