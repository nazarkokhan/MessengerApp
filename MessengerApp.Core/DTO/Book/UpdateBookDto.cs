using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MessengerApp.Core.DTO.Book
{
    public class UpdateBookDto
    {
        public UpdateBookDto(int id, string name, IEnumerable<int> authorIds)
        {
            Id = id;
            Name = name;
            AuthorIds = authorIds;
        }

        [Required] 
        [Range(0, int.MaxValue)] 
        public int Id { get; }

        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string Name { get; }

        [Required] 
        public IEnumerable<int> AuthorIds { get; }
    }
}