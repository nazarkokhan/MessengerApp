using System.Threading.Tasks;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Chat;
using MessengerApp.Core.DTO.Message;
using MessengerApp.Core.ResultConstants;
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
            string? search,
            int page,
            int items
        ) =>
            _unitOfWork.Chats.GetChatAsync(search, page, items);

        public Task<Result<Pager<ChatDto>>> GetUserChatsPageAsync(
            int userId,
            string? search,
            int page,
            int items
        ) =>
            _unitOfWork.Chats.GetUserChatsPageAsync(userId, search, page, items);

        public async Task<Result<ChatDto>> AddUserInChatAsync(
            int userId, AddUserInChatDto addUserInChatDto
        )
        {
            var result = await _unitOfWork.Chats.AddUserInChatAsync(userId, addUserInChatDto);

            if (!result.Success)
                return result;

            await _unitOfWork.Message.CreateMessageAsync(
                addUserInChatDto.UserId,
                addUserInChatDto.ChatId,
                new CreateMessageDto(
                    $"{(await _unitOfWork.Users.GetUserAsync(addUserInChatDto.UserId)).Data.UserName}" +
                     " joined this chat."
                )
            );

            return result;
        }

        public Task<Result<ChatDto>> GetChatAsync(
            int id
        ) =>
            _unitOfWork.Chats.GetChatAsync(id);

        public Task<Result<ChatDto>> CreateChatAsync(
            int userId, CreateChatDto createChatDto
        ) =>
            _unitOfWork.Chats.CreateChatAsync(userId, createChatDto);

        public Task<Result<ChatDto>> EditChatAsync(
            int userId,
            EditChatDto editChatDto
        ) =>
            _unitOfWork.Chats.EditChatAsync(userId, editChatDto);

        public Task<Result> DeleteChatAsync(
            int userId,
            int id
        ) =>
            _unitOfWork.Chats.DeleteChatAsync(userId, id);
    }
}