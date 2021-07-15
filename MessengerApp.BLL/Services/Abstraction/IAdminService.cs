using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Entities.Authorization;

namespace MessengerApp.BLL.Services.Abstraction
{
    public interface IAdminService
    {
        Task<Result<Pager<User>>> GetUsersPageAsync(string? search, int page, int items);

        Task<Result<User>> GetUserAsync([Range(0, int.MaxValue)] int id);

        Task<Result<User>> EditUserAsync(EditUserDto editUserDto);

        Task<Result> DeleteUserAsync([Range(0, int.MaxValue)] int id);
    }
}