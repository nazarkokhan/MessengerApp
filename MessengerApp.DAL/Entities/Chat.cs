using System.Collections.Generic;
using System.Runtime.Serialization;
using MessengerApp.DAL.Entities.Abstract;
// ReSharper disable All
#pragma warning disable 8618

namespace MessengerApp.DAL.Entities
{
    public class Chat : EntityBase
    {
        public string Name { get; set; }

        public ICollection<Message> Messages { get; set; }
        
        [DataMember] 
        public ICollection<ChatUser> ChatUsers { get; set; }
    }
}