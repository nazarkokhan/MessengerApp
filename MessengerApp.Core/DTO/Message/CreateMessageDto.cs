namespace MessengerApp.Core.DTO.Message
{
    public class CreateMessageDto
    {
        public CreateMessageDto(string body)
        {
            Body = body;
        }

        public string Body { get; }
    }
}