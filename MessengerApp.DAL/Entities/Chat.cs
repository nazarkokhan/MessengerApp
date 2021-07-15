using System.Collections.Generic;
using System.Runtime.Serialization;
using MessengerApp.DAL.Entities.Abstract;

namespace MessengerApp.DAL.Entities
{
    public class Chat : EntityBase
    {
        public string Name { get; set; }

        [DataMember] 
        public ICollection<ChatUser> ChatUsers { get; set; }
    }
}