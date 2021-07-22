namespace MessengerApp.Core.DTO.User
{
    public class UserDto
    {
        public UserDto(int id, string userName, string about, 
            string email)
        {
            Id = id;
            UserName = userName;
            About = about;
            Email = email;
        }

        public int Id { get; }
        
        public string UserName { get; }

        public string About { get; }

        public string Email { get; }
    }
}