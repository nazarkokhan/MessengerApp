using System.Collections.Generic;
using System.Runtime.Serialization;
using MessengerApp.DAL.Entities.Abstract;

namespace MessengerApp.DAL.Entities
{
    public class Author : EntityBase
    {
        public string Name { get; set; }

        [DataMember] 
        public ICollection<AuthorBook> AuthorBooks { get; set; }
    }
}