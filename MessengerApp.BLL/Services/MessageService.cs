﻿using System.Threading.Tasks;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Message;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Repository.Abstraction;

namespace MessengerApp.BLL.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MessageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Result<Pager<MessageDto>>> GetMessagesInChatPageAsync(
            int chatId, string? search, int page, int items)
            => _unitOfWork.Message.GetMessagesInChatPageAsync(chatId, search, page, items);

        public Task<Result<MessageDto>> EditMessageAsync(
            int userId, int messageId, EditMessageDto editMessageDto)
            => _unitOfWork.Message.EditMessageAsync(userId, messageId, editMessageDto);

        public Task<Result> DeleteMessageAsync(
            int userId, int messageId)
            => _unitOfWork.Message.DeleteMessageAsync(userId, messageId);
    }
}