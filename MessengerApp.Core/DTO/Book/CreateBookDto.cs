using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MessengerApp.Core.DTO.Book
{
    public class CreateBookDto
    {
        public CreateBookDto(string name, IEnumerable<int> authorIds)
        {
            Name = name;
            AuthorIds = authorIds;
        }

        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string Name { get; }

        [Required] 
        public IEnumerable<int> AuthorIds { get; }
    }
}