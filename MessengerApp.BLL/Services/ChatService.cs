using System.Threading.Tasks;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Chat;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Repository.Abstraction;

namespace MessengerApp.BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Result<Pager<ChatDto>>> GetChatsPageAsync(
            string? search, int page, int items
        ) =>
            _unitOfWork.Chats.GetChatAsync(search, page, items);

        public Task<Result<Pager<ChatDto>>> GetUserChatsPageAsync(int userId, string? search, int page, int items
        ) =>
            _unitOfWork.Chats.GetUserChatsPageAsync(userId, search, page, items);

        public Task<Result<ChatDto>> GetChatAsync(
            int chatId
        ) =>
            _unitOfWork.Chats.GetChatAsync(chatId);

        public Task<Result<ChatDto>> EditChatAsync(
            int userId, int chatId, EditChatDto editChatDto
        ) =>
            _unitOfWork.Chats.EditChatAsync(userId, chatId, editChatDto);

        public Task<Result> DeleteChatAsync(
            int userId, int chatId
        ) =>
            _unitOfWork.Chats.DeleteChatAsync(userId, chatId);
    }
}