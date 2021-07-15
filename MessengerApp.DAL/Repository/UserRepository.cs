using System;
using System.Linq;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.Extensions;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.EF;
using MessengerApp.DAL.Entities.Authorization;
using MessengerApp.DAL.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace MessengerApp.DAL.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MsgContext _db;

        public UserRepository(MsgContext context)
        {
            _db = context;
        }
        
        public async Task<Result<Pager<User>>> GetUsersPageAsync(
            string? search, int page, int items)
        {
            try
            {
                var totalCount = await _db.Users
                    .CountAsync();

                var userEntities = _db.Users
                    .OrderBy(a => a.Id)
                    .TakePage(page, items);

                if (!string.IsNullOrWhiteSpace(search))
                    userEntities = userEntities
                        .Where(u => u.UserName.Contains(search) || u.Email.Contains(search));

                return Result<Pager<User>>.CreateSuccess(
                    new Pager<User>(
                        await userEntities.ToListAsync(),
                        totalCount
                    )
                );
            }
            catch (Exception e)
            {
                return Result<Pager<User>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
        
        public async Task<Result<Pager<User>>> GetUsersInChatAsync(
            int chatId, string? search, int page, int items)
        {
            try
            {
                var totalCount = await _db.ChatUsers
                    .Where(cu => cu.ChatId == chatId)
                    .CountAsync();

                var userEntities = _db.ChatUsers
                    .Where(cu => cu.ChatId == chatId)
                    .Select(cu => cu.User)
                    .OrderBy(u => u.Id)
                    .TakePage(page, items);

                if (!string.IsNullOrWhiteSpace(search))
                    userEntities = userEntities
                        .Where(u => u.UserName.Contains(search) || u.Email.Contains(search));

                return Result<Pager<User>>.CreateSuccess(
                    new Pager<User>(
                        await userEntities.ToListAsync(),
                        totalCount
                    )
                );
            }
            catch (Exception e)
            {
                return Result<Pager<User>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
        
        public async Task<Result<User>> GetUserAsync(int id)
        {
            try
            {
                var userEntity = await _db.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                return userEntity is null
                    ? Result<User>.CreateFailed(
                        AccountResultConstants.UserNotFound,
                        new NullReferenceException()
                    )
                    : Result<User>.CreateSuccess(userEntity);
            }
            catch (Exception e)
            {
                return Result<User>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<User>> EditUserAsync(EditUserDto userDto)
        {
            try
            {
                var userEntity = await _db.Users
                    .FirstOrDefaultAsync(u => u.Id == userDto.Id);

                if (userEntity is null)
                    return Result<User>.CreateFailed(
                        AccountResultConstants.UserNotFound,
                        new NullReferenceException()
                    );

                userEntity.Email = userDto.NewEmail;

                userEntity.UserName = userDto.NewEmail;

                userEntity.Age = userDto.NewAge;

                await _db.SaveChangesAsync();
                    
                return Result<User>.CreateSuccess(userEntity);
            }
            catch (Exception e)
            {
                return Result<User>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result> DeleteUserAsync(int id)
        {
            try
            {
                var userEntity = await _db.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (userEntity is null)
                    return Result.CreateFailed(
                        AccountResultConstants.UserNotFound,
                        new NullReferenceException()
                    );

                _db.Users.Remove(userEntity);

                await _db.SaveChangesAsync();

                return Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
        
        public async Task<Result<bool>> UserExistsAsync(string email)
        {
            try
            {
                return Result<bool>.CreateSuccess(
                    await _db.Users.AnyAsync(u => u.Email == email)
                );
            }
            catch (Exception e)
            {
                return Result<bool>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
    }
}