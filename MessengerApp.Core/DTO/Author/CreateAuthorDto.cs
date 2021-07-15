using System.ComponentModel.DataAnnotations;

namespace MessengerApp.Core.DTO.Author
{
    public class CreateAuthorDto
    {
        public CreateAuthorDto(string name)
        {
            Name = name;
        }

        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string Name { get; }
    }
}