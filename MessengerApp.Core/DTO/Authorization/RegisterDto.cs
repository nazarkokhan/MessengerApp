using System.ComponentModel.DataAnnotations;

namespace MessengerApp.Core.DTO.Authorization
{
    public class RegisterDto
    {
        public RegisterDto(string userName, string email, string password)
        {
            UserName = userName;
            Email = email;
            Password = password;
        }

        [Required]
        [DataType(DataType.Text)]
        public string UserName { get; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; }
    }
}