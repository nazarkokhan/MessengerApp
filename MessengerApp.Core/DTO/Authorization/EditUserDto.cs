namespace MessengerApp.Core.DTO.Authorization
{
    public class EditUserDto
    {
        public EditUserDto(string newEmail, int newAge, string newPassword, int id)
        {
            NewEmail = newEmail;
            NewAge = newAge;
            NewPassword = newPassword;
            Id = id;
        }

        public string NewEmail { get; }

        public int NewAge { get; }

        public string NewPassword { get; }

        public int Id { get; }
    }
}