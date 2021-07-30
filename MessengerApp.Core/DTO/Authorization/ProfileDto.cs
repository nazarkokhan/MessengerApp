namespace MessengerApp.Core.DTO.Authorization
{
    public class ProfileDto
    {
        public ProfileDto(int id, string email, string userName, string? about)
        {
            Id = id;
            Email = email;
            UserName = userName;
            About = about;
        }

        public int Id { get; }

        public string Email { get; }

        public string UserName { get; }

        public string About { get; }
    }
}