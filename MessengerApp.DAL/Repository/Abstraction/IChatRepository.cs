﻿using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Chat;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;

namespace MessengerApp.DAL.Repository.Abstraction
{
    public interface IChatRepository
    {
        Task<Result<Pager<ChatDto>>> GetChatAsync(
            string? search, int page, int items);
        
        Task<Result<Pager<ChatDto>>> GetUserChatsPageAsync(
            int userId, string? search, int page, int items);

        Task<Result<ChatDto>> GetChatAsync(
            int chatId);

        Task<Result<ChatDto>> EditChatAsync(
            int userId, int chatId, EditChatDto editChatDto);
        
        Task<Result> DeleteChatAsync(
            int userId, int chatId);
    }
}