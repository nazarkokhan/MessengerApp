using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.User;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;

namespace MessengerApp.BLL.Services.Abstraction
{
    public interface IAdminService
    {
        Task<Result<Pager<UserDto>>> GetUsersPageAsync(
            string? search, int page, int items);

        Task<Result<UserDto>> GetUserAsync(
            int id);

        Task<Result<UserDto>> EditUserAsync(
            EditUserDto editUserDto);

        Task<Result> DeleteUserAsync(
            int id);
    }
}