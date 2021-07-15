using System.Collections.Generic;
using MessengerApp.DAL.Entities.Abstract;

namespace MessengerApp.DAL.Entities
{
    public class Book : EntityBase
    {
        public string Name { get; set; }

        public ICollection<AuthorBook> AuthorBooks { get; set; }
    }
}