using System.Collections.Generic;

namespace MessengerApp.Core.DTO.User
{
    public class UserDto
    {
        public UserDto(string userName, string about, 
            string email, ICollection<int> contactIds, 
            ICollection<long> messageIds, ICollection<int> chatIds)
        {
            UserName = userName;
            About = about;
            Email = email;
            ContactIds = contactIds;
            MessageIds = messageIds;
            ChatIds = chatIds;
        }

        public string UserName { get; }

        public string About { get; }

        public string Email { get; }

        public ICollection<int> ContactIds { get; }

        public ICollection<long> MessageIds { get; }

        public ICollection<int> ChatIds { get; }
    }
}