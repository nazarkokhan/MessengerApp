using System;
using System.Linq;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.DTO.User;
using MessengerApp.Core.Extensions;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.EF;
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

        public async Task<Result<Pager<UserDto>>> GetUsersPageAsync(
            string? search, int page, int items)
        {
            try
            {
                var userEntities = _db.Users
                    .OrderBy(a => a.UserName)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                    userEntities = userEntities
                        .Where(u => u.UserName.Contains(search) || u.Email.Contains(search));

                return Result<Pager<UserDto>>.CreateSuccess(
                    new Pager<UserDto>(
                        await userEntities
                            .TakePage(page, items)
                            .Select(u => u.MapUserDto())
                            .ToListAsync(),
                        await userEntities.CountAsync()
                    )
                );
            }
            catch (Exception e)
            {
                return Result<Pager<UserDto>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<Pager<UserDto>>> GetUsersInChatAsync(
            int userid, int chatId, string? search, int page, int items)
        {
            try
            {
                var userEntities = _db.ChatUsers
                    .Include(cu => cu.User)
                    .OrderBy(cu => cu.User.UserName)
                    .Where(cu => cu.ChatId == chatId)
                    .Select(cu => cu.User);
                
                if (!string.IsNullOrWhiteSpace(search))
                    userEntities = userEntities
                        .Where(u => u.UserName.Contains(search) || u.Email.Contains(search));

                return Result<Pager<UserDto>>.CreateSuccess(
                    new Pager<UserDto>(
                        await userEntities
                            .TakePage(page, items)
                            .Select(u => u.MapUserDto())
                            .ToListAsync(),
                        await userEntities.CountAsync()
                    )
                );
            }
            catch (Exception e)
            {
                return Result<Pager<UserDto>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<UserDto>> GetUserAsync(
            int id)
        {
            try
            {
                var userEntity = await _db.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                return userEntity is null
                    ? Result<UserDto>.CreateFailed(
                        UserResultConstants.UserNotFound,
                        new NullReferenceException()
                    )
                    : Result<UserDto>.CreateSuccess(userEntity.MapUserDto());
            }
            catch (Exception e)
            {
                return Result<UserDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
        
        public async Task<Result<UserDto>> EditUserAsync(
            int id, EditUserDto editUserDto)
        {
            try
            {
                var userEntity = await _db.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (userEntity is null)
                    return Result<UserDto>.CreateFailed(
                        UserResultConstants.UserNotFound,
                        new NullReferenceException()
                    );

                userEntity.MapEditUserDto(editUserDto);

                await _db.SaveChangesAsync();

                return Result<UserDto>.CreateSuccess(userEntity.MapUserDto());
            }
            catch (Exception e)
            {
                return Result<UserDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<UserDto>> EditUserByAdminAsync(
            EditUserByAdminDto editUserByAdminDto)
        {
            try
            {
                var userEntity = await _db.Users
                    .FirstOrDefaultAsync(u => u.Id == editUserByAdminDto.Id);

                if (userEntity is null)
                    return Result<UserDto>.CreateFailed(
                        UserResultConstants.UserNotFound,
                        new NullReferenceException()
                    );

                userEntity.MapEditUserByAdminDto(editUserByAdminDto);

                await _db.SaveChangesAsync();

                return Result<UserDto>.CreateSuccess(userEntity.MapUserDto());
            }
            catch (Exception e)
            {
                return Result<UserDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result> DeleteUserAsync(
            int id)
        {
            try
            {
                var userEntity = await _db.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (userEntity is null)
                    return Result.CreateFailed(
                        UserResultConstants.UserNotFound,
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
        

        public async Task<Result<bool>> UserExistsAsync(
            string email)
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