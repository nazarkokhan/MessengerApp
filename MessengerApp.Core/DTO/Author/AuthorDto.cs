namespace MessengerApp.Core.DTO.Author
{
    public class AuthorDto
    {
        public AuthorDto(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }

        public string Name { get; }
    }
}