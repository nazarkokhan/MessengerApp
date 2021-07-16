using System;
using System.Linq;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.Extensions;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.EF;
using MessengerApp.DAL.Entities;
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

        public async Task<Result<Pager<Message>>> GetMessagesInChatPageAsync(
            int chatId, string? search, int page, int items)
        {
            try
            {
                var totalCount = await _db.Messages
                    .Where(m => m.ChatId == chatId)
                    .CountAsync();

                var messages = _db.Messages
                    .OrderByDescending(m => m.DateTime)
                    .TakePage(page, items);

                if (!string.IsNullOrWhiteSpace(search))
                    messages = messages
                        .Where(m => m.Body.Contains(search));

                return Result<Pager<Message>>.CreateSuccess(
                    new Pager<Message>(
                        await messages.ToListAsync(),
                        totalCount
                    )
                );
            }
            catch (Exception e)
            {
                return Result<Pager<Message>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
        
        public async Task<Result<Pager<Message>>> EditMessageAsync(
            int chatId, string? search, int page, int items)
        {
            try
            {
                var totalCount = await _db.Messages
                    .Where(m => m.ChatId == chatId)
                    .CountAsync();

                var messages = _db.Messages
                    .OrderByDescending(m => m.DateTime)
                    .TakePage(page, items);

                if (!string.IsNullOrWhiteSpace(search))
                    messages = messages
                        .Where(m => m.Body.Contains(search));

                return Result<Pager<Message>>.CreateSuccess(
                    new Pager<Message>(
                        await messages.ToListAsync(),
                        totalCount
                    )
                );
            }
            catch (Exception e)
            {
                return Result<Pager<Message>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
    }
}