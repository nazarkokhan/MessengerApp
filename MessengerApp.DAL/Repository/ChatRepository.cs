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
    public class ChatRepository : IChatRepository
    {
        private readonly MsgContext _db;

        public ChatRepository(MsgContext context)
        {
            _db = context;
        }
        
        public async Task<Result<Pager<Chat>>> GetChatsPageAsync(
            string? search, int page, int items)
        {
            try
            {
                var totalCount = await _db.Chats
                    .CountAsync();

                var chatEntities = _db.Chats
                    .OrderBy(c => c.Id)
                    .TakePage(page, items);

                if (!string.IsNullOrWhiteSpace(search))
                    chatEntities = chatEntities
                        .Where(c => c.Name.Contains(search));

                return Result<Pager<Chat>>.CreateSuccess(
                    new Pager<Chat>(
                        await chatEntities.ToListAsync(),
                        totalCount
                    )
                );
            }
            catch (Exception e)
            {
                return Result<Pager<Chat>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
        
        public async Task<Result<Pager<Chat>>> GetСhatsPerUserPageAsync(
            int userId, string? search, int page, int items)
        {
            try
            {
                var totalCount = await _db.ChatUsers
                    .Where(cu => cu.UserId == userId)
                    .CountAsync();

                var chatEntities = _db.ChatUsers
                    .Where(cu => cu.UserId == userId)
                    .Select(cu => cu.Chat)
                    .OrderBy(c => c.Id)
                    .TakePage(page, items);

                if (!string.IsNullOrWhiteSpace(search))
                    chatEntities = chatEntities
                        .Where(c => c.Name.Contains(search));

                return Result<Pager<Chat>>.CreateSuccess(
                    new Pager<Chat>(
                        await chatEntities.ToListAsync(),
                        totalCount
                    )
                );
            }
            catch (Exception e)
            {
                return Result<Pager<Chat>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        // public async Task DeleteChat(int userId, )
    }
}