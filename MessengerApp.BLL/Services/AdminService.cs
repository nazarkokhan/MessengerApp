using System;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Repository.Abstraction;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.DTO.User;
using MessengerApp.DAL.Entities.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MessengerApp.BLL.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<User> _userManager;

        private readonly IUnitOfWork _unitOfWork;

        public AdminService(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public Task<Result<Pager<UserDto>>> GetUsersPageAsync(
            string? search, int page, int items
        ) =>
            _unitOfWork.Users.GetUsersPageAsync(search, page, items);

        public Task<Result<UserDto>> GetUserAsync(
            int id
        ) =>
            _unitOfWork.Users.GetUserAsync(id);

        public async Task<Result<UserDto>> EditUserAsync(
            EditUserByAdminDto editUserByAdminDto)
        {
            try
            {
                var editUserResult = await _unitOfWork.Users.EditUserByAdminAsync(editUserByAdminDto);

                if (!editUserResult.Success)
                    return editUserResult;

                var userEntity = await _userManager.FindByEmailAsync(editUserResult.Data.Email);

                var removePassword = await _userManager.RemovePasswordAsync(userEntity);

                if (!removePassword.Succeeded)
                    return Result<UserDto>.CreateFailed(UserResultConstants.ErrorRemovingPassword);

                var addPass = await _userManager
                    .AddPasswordAsync(userEntity, editUserByAdminDto.NewPassword);

                return !addPass.Succeeded
                    ? Result<UserDto>.CreateFailed(UserResultConstants.ErrorAddingPassword)
                    : Result<UserDto>.CreateSuccess(userEntity.MapUserDto());
            }
            catch (Exception e)
            {
                return Result<UserDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public Task<Result> DeleteUserAsync(
            int id
        ) =>
            _unitOfWork.Users.DeleteUserAsync(id);
    }
}