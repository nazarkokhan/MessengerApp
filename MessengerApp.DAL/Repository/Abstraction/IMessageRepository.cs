using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Message;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;

namespace MessengerApp.DAL.Repository.Abstraction
{
    public interface IMessageRepository
    {
        Task<Result<Pager<MessageDto>>> GetMessagesInChatPageAsync(
            int chatId, string? search, int page, int items);

        Task<Result<MessageDto>> EditMessageAsync(
            int userId, int messageId, EditMessageDto editMessageDto);

        Task<Result> DeleteMessageAsync(
            int userId, int messageId);
    }
}