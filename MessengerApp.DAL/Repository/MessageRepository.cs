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
            int chatId, string? search, int page, int items)
        {
            try
            {
                var totalCount = await _db.Messages
                    .Where(m => m.ChatId == chatId)
                    .CountAsync();

                var messages = _db.Messages
                    .Select(m => m.MapMessageDto())
                    .OrderByDescending(m => m.DateTime)
                    .TakePage(page, items);

                if (!string.IsNullOrWhiteSpace(search))
                    messages = messages
                        .Where(m => m.Body.Contains(search));

                return Result<Pager<MessageDto>>.CreateSuccess(
                    new Pager<MessageDto>(
                        await messages.ToListAsync(),
                        totalCount
                    )
                );
            }
            catch (Exception e)
            {
                return Result<Pager<MessageDto>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<MessageDto>> EditMessageAsync(
            int userId, int messageId, EditMessageDto editMessageDto)
        {
            try
            {
                var message = await _db.Messages
                    .FirstOrDefaultAsync(m => m.Id == messageId);
                
                if(message is null)
                    return Result<MessageDto>.CreateFailed(MessageResultConstants.MessageNotFount);

                message.MapEditMessageDto(editMessageDto);
                
                if (message.UserId != userId)
                    return Result<MessageDto>.CreateFailed(
                        MessageResultConstants.NoRulesToEditMessage,
                        new UnauthorizedAccessException()
                    );

                return Result<MessageDto>.CreateSuccess(message.MapMessageDto());
            }
            catch (Exception e)
            {
                return Result<MessageDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result> DeleteMessageAsync(
            int userId, int messageId)
        {
            try
            {
                var message = await _db.Messages
                    .FirstOrDefaultAsync(m => m.Id == messageId);
                
                if(message is null)
                    return Result.CreateFailed(MessageResultConstants.MessageNotFount);

                if (message.UserId != userId)
                    return Result.CreateFailed(
                        MessageResultConstants.NoRulesToEditMessage,
                        new UnauthorizedAccessException()
                    );

                _db.Messages.Remove(message);

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