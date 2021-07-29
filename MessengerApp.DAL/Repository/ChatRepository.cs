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
                var chatEntities = _db.Chats
                    .OrderBy(c => c.Name)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                    chatEntities = chatEntities
                        .Where(c => c.Name.Contains(search));

                return Result<Pager<ChatDto>>.CreateSuccess(
                    new Pager<ChatDto>(
                        await chatEntities
                            .TakePage(page, items)
                            .Select(c => c.MapChatDto())
                            .ToListAsync(),
                        await chatEntities.CountAsync()
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
                var chatEntities = _db.ChatUsers
                    .Include(cu => cu.Chat)
                    .OrderBy(cu => cu.Chat.Name)
                    .Where(cu => cu.UserId == userId)
                    .Select(cu => cu.Chat);

                if (!string.IsNullOrWhiteSpace(search))
                    chatEntities = chatEntities
                        .Where(c => c.Name.Contains(search));

                return Result<Pager<ChatDto>>.CreateSuccess(
                    new Pager<ChatDto>(
                        await chatEntities
                            .TakePage(page, items)
                            .Select(c => c.MapChatDto())
                            .ToListAsync(),
                        await chatEntities.CountAsync()
                    )
                );
            }
            catch (Exception e)
            {
                return Result<Pager<ChatDto>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<ChatDto>> AddUserInChatAsync(
            int userId, AddUserInChatDto addUserInChatDto)
        {
            try
            {
                var chatEntity = await _db.Chats
                    .Include(c => c.ChatUsers)
                    .FirstOrDefaultAsync(c => c.Id == addUserInChatDto.ChatId);

                if (chatEntity is null)
                    return Result<ChatDto>.CreateFailed(ChatResultConstants.ChatNotFound);

                if (chatEntity.AdminId != userId)
                    return Result<ChatDto>.CreateFailed(CommonResultConstants.NoRules);

                var chatUserEntity = addUserInChatDto.MapChatUser();

                if (chatEntity.ChatUsers
                        .FirstOrDefault(cu => cu.UserId == userId) is null)
                    return Result<ChatDto>.CreateFailed(ChatResultConstants.UserAlreadyInChat);

                chatEntity.ChatUsers.Add(chatUserEntity);

                await _db.SaveChangesAsync();
                
                return Result<ChatDto>.CreateSuccess(chatEntity.MapChatDto());
            }
            catch (Exception e)
            {
                return Result<ChatDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<ChatDto>> GetChatAsync(
            int id)
        {
            try
            {
                var chatEntity = await _db.Chats.FirstOrDefaultAsync(c => c.Id == id);

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
            int userId, EditChatDto editChatDto)
        {
            try
            {
                var chatEntity = await _db.Chats
                    .Include(c => c.ChatUsers)
                    .FirstOrDefaultAsync(c => c.Id == editChatDto.Id);

                if (chatEntity is null)
                    Result.CreateFailed(ChatResultConstants.ChatNotFound, new NullReferenceException());

                if (chatEntity!.AdminId != userId)
                    return Result<ChatDto>.CreateFailed(CommonResultConstants.NoRules);

                if (!chatEntity.ChatUsers.Select(cu => cu.UserId).Contains(editChatDto.AdminId))
                    editChatDto.AdminId = chatEntity.AdminId;

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
            int userId, int id)
        {
            try
            {
                var chatEntity = await _db.Chats.FirstOrDefaultAsync(c => c.Id == id);

                if (chatEntity is null)
                    Result.CreateFailed(ChatResultConstants.ChatNotFound, new NullReferenceException());

                if (chatEntity!.AdminId != userId)
                    return Result.CreateFailed(CommonResultConstants.NoRules);

                _db.Chats.Remove(chatEntity!);

                await _db.SaveChangesAsync();

                return Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
    }
}