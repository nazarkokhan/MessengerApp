using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace MessengerApp.Core.DTO.Authorization.Reset
{
    public class ResetEmailDto
    {
        public ResetEmailDto(string newEmail, string confirmNewEmail)
        {
            NewEmail = newEmail;
            ConfirmNewEmail = confirmNewEmail;
        }

        [DataType(DataType.EmailAddress)]
        public string NewEmail { get; }

        [DataMember]
        [Compare("NewEmail", ErrorMessage = "Emails have to be equal")]
        public string ConfirmNewEmail { get; }
    }
}