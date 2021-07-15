using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace MessengerApp.Core.DTO.Authorization.Reset
{
    public class TokenPasswordDto
    {
        public TokenPasswordDto(string email, string newPassword, string confirmNewPassword, string token)
        {
            Email = email;
            NewPassword = newPassword;
            ConfirmNewPassword = confirmNewPassword;
            Token = token;
        }

        [DataType(DataType.EmailAddress)] 
        public string Email { get; }

        [DataType(DataType.Password)] 
        public string NewPassword { get; }

        [DataMember]
        [Compare("NewPassword", ErrorMessage = "Passwords have to be equal")]
        public string ConfirmNewPassword { get; }

        public string Token { get; }
    }
}