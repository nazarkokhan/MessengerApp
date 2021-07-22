using System;
using System.Linq;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Message;
using MessengerApp.Core.Extensions;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.EF;
using MessengerApp.DAL.Entities;
using MessengerApp.DAL.Extensions;
using MessengerApp.DAL.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace MessengerApp.DAL.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MsgContext _db;

        public MessageRepository(MsgContext context)
        {
            _db = context;
        }

        public async Task<Result<Pager<MessageDto>>> GetMessagesInChatPageAsync(
            int userId, int chatId, string? search, int page, int items)
        {
            try
            {
                var messageEntities = _db.Messages
                    .OrderBy(m => m.DateTime)
                    .Where(m => m.ChatId == chatId)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                    messageEntities = messageEntities
                        .Where(m => m.Body.Contains(search));

                return Result<Pager<MessageDto>>.CreateSuccess(
                    new Pager<MessageDto>(
                        await messageEntities
                            .TakePage(page, items)
                            .Select(m => m.MapMessageDto())
                            .ToListAsync(),
                        await messageEntities.CountAsync()
                    )
                );
            }
            catch (Exception e)
            {
                return Result<Pager<MessageDto>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<MessageDto>> CreateMessageAsync(
            int userId, int chatId, CreateMessageDto createMessageDto)
        {
            try
            {
                var chatUser = await _db.ChatUsers.FindAsync(new ChatUser
                {
                    UserId = userId,
                    ChatId = chatId
                });
                
                if(chatUser is null)
                    return Result<MessageDto>.CreateFailed(CommonResultConstants.NoRules);

                var messageEntity = createMessageDto.MapMessage(userId, chatId);

                await _db.AddAsync(messageEntity);

                await _db.SaveChangesAsync();

                if (messageEntity.Id < 1)
                    return Result<MessageDto>.CreateFailed(
                        MessageResultConstants.ErrorAddingMessage
                    );

                return Result<MessageDto>.CreateSuccess(messageEntity.MapMessageDto());
            }
            catch (Exception e)
            {
                return Result<MessageDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<MessageDto>> EditMessageAsync(
            int userId, EditMessageDto editMessageDto)
        {
            try
            {
                var messageEntity = await _db.Messages
                    .FirstOrDefaultAsync(m => m.Id == editMessageDto.Id);

                if (messageEntity is null)
                    return Result<MessageDto>.CreateFailed(MessageResultConstants.MessageNotFount);
                
                if (messageEntity.UserId != userId)
                    return Result<MessageDto>.CreateFailed(CommonResultConstants.NoRules);
                
                messageEntity.MapEditMessageDto(editMessageDto);
                
                await _db.SaveChangesAsync();
                
                return Result<MessageDto>.CreateSuccess(messageEntity.MapMessageDto());
            }
            catch (Exception e)
            {
                return Result<MessageDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result> DeleteMessageAsync(
            int userId, long messageId)
        {
            try
            {
                var messageEntity = await _db.Messages
                    .FirstOrDefaultAsync(m => m.Id == messageId);

                if (messageEntity is null)
                    return Result.CreateFailed(MessageResultConstants.MessageNotFount);

                if (messageEntity.UserId != userId)
                    return Result.CreateFailed(CommonResultConstants.NoRules);

                _db.Messages.Remove(messageEntity);

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