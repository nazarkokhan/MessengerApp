using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Entities.Authorization;

namespace MessengerApp.DAL.Repository.Abstraction
{
    public interface IUserRepository
    {
        Task<Result<Pager<User>>> GetUsersPageAsync(
            string? search, int page, int items);

        Task<Result<Pager<User>>> GetUsersInChatAsync(
            int chatId, string? search, int page, int items);
        
        Task<Result<User>> GetUserAsync(int id);
        
        Task<Result<User>> EditUserAsync(EditUserDto userDto);

        Task<Result> DeleteUserAsync(int id);
        
        Task<Result<bool>> UserExistsAsync(string email);
    }
}