using System.ComponentModel.DataAnnotations;

namespace MessengerApp.Core.DTO.Authorization
{
    public class RegisterDto
    {
        public RegisterDto(string email, string password, int age)
        {
            Email = email;
            Password = password;
            Age = age;
        }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; }

        [Required] 
        [Range(0, 150)] 
        public int Age { get; }
    }
}