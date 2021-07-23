using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.DTO.User;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApp.BLL.Services.Abstraction
{
    public interface IAdminService
    {
        Task<Result<Pager<UserDto>>> GetUsersPageAsync(
            string? search, int page, int items);

        Task<Result<UserDto>> GetUserAsync(
            [FromQuery] [Range(1, int.MaxValue)] int id);

        Task<Result<UserDto>> EditUserAsync(
            EditUserByAdminDto editUserByAdminDto);

        Task<Result> DeleteUserAsync(
            int id);
    }
}