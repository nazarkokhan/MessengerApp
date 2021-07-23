using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.DTO.User;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;

namespace MessengerApp.DAL.Repository.Abstraction
{
    public interface IUserRepository
    {
        Task<Result<Pager<UserDto>>> GetUsersPageAsync(
            string? search, int page, int items);

        Task<Result<Pager<UserDto>>> GetUsersInChatAsync(
            int userId, int chatId, string? search, int page, int items);
        
        Task<Result<UserDto>> GetUserAsync(
            int id);

        Task<Result<UserDto>> EditUserAsync(
            int id, EditUserDto editUserDto);
        
        Task<Result<UserDto>> EditUserByAdminAsync(
            EditUserByAdminDto editUserByAdminDto);

        Task<Result> DeleteUserAsync(
            int id);
        
        Task<Result<bool>> UserExistsAsync(
            string email);
    }
}