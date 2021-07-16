using System;
using System.Linq;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Chat;
using MessengerApp.Core.Extensions;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.EF;
using MessengerApp.DAL.Extensions;
using MessengerApp.DAL.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace MessengerApp.DAL.Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly MsgContext _db;

        public ChatRepository(MsgContext context)
        {
            _db = context;
        }

        public async Task<Result<Pager<ChatDto>>> GetChatAsync(
            string? search, int page, int items)
        {
            try
            {
                var totalCount = await _db.Chats
                    .CountAsync();

                var chatEntities = _db.Chats
                    .Select(c => c.MapChatDto())
                    .OrderBy(c => c.Name)
                    .TakePage(page, items);

                if (!string.IsNullOrWhiteSpace(search))
                    chatEntities = chatEntities
                        .Where(c => c.Name.Contains(search));

                return Result<Pager<ChatDto>>.CreateSuccess(
                    new Pager<ChatDto>(
                        await chatEntities.ToListAsync(),
                        totalCount
                    )
                );
            }
            catch (Exception e)
            {
                return Result<Pager<ChatDto>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<Pager<ChatDto>>> GetUserChatsPageAsync(
            int userId, string? search, int page, int items)
        {
            try
            {
                var totalCount = await _db.ChatUsers
                    .Where(cu => cu.UserId == userId)
                    .CountAsync();

                var chatEntities = _db.ChatUsers
                    .Where(cu => cu.UserId == userId)
                    .Select(cu => cu.Chat.MapChatDto())
                    .OrderBy(c => c.Name)
                    .TakePage(page, items);

                if (!string.IsNullOrWhiteSpace(search))
                    chatEntities = chatEntities
                        .Where(c => c.Name.Contains(search));

                return Result<Pager<ChatDto>>.CreateSuccess(
                    new Pager<ChatDto>(
                        await chatEntities.ToListAsync(),
                        totalCount
                    )
                );
            }
            catch (Exception e)
            {
                return Result<Pager<ChatDto>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<ChatDto>> GetChatAsync(
            int chatId)
        {
            try
            {
                var chatEntity = await _db.Chats.FirstOrDefaultAsync(c => c.Id == chatId);

                return chatEntity is null
                    ? Result<ChatDto>.CreateFailed(ChatResultConstants.ChatNotFound, new NullReferenceException())
                    : Result<ChatDto>.CreateSuccess(chatEntity.MapChatDto());
            }
            catch (Exception e)
            {
                return Result<ChatDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<ChatDto>> CreateChatAsync(
            int userId, CreateChatDto createChatDto)
        {
            try
            {
                var chatEntity = createChatDto.MapChat(userId);

                await _db.Chats.AddAsync(chatEntity);

                await _db.SaveChangesAsync();

                return Result<ChatDto>.CreateSuccess(
                    chatEntity.MapChatDto()
                );
            }
            catch (Exception e)
            {
                return Result<ChatDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<ChatDto>> EditChatAsync(
            int userId, int chatId, EditChatDto editChatDto)
        {
            try
            {
                var chatEntity = await _db.Chats.FirstOrDefaultAsync(c => c.Id == chatId);

                if (chatEntity is null)
                    Result.CreateFailed(ChatResultConstants.ChatNotFound, new NullReferenceException());

                chatEntity!.MapEditChatDto(editChatDto);

                await _db.SaveChangesAsync();

                return Result<ChatDto>.CreateSuccess(chatEntity.MapChatDto());
            }
            catch (Exception e)
            {
                return Result<ChatDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result> DeleteChatAsync(
            int userId, int chatId)
        {
            try
            {
                var chatEntity = await _db.Chats.FirstOrDefaultAsync(c => c.Id == chatId);

                if (chatEntity is null)
                    Result.CreateFailed(ChatResultConstants.ChatNotFound, new NullReferenceException());

                _db.Chats.Remove(chatEntity!);

                return Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
    }
}