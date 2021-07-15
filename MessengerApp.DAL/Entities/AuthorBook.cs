using MessengerApp.DAL.Entities.Abstract;

namespace MessengerApp.DAL.Entities
{
    public class AuthorBook : EntityBase
    {
        public int BookId { get; set; }
        
        public Author Author { get; set; }

        public int AuthorId { get; set; }
        
        public Book Book { get; set; }
    }
}