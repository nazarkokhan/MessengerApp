// ReSharper disable All
namespace MessengerApp.Core.DTO.Message
{
    public class EditMessageDto
    {
        public EditMessageDto(long id, string body)
        {
            Id = id;
            Body = body;
        }

        public long Id { get; }

        public string Body { get; }
    }
}