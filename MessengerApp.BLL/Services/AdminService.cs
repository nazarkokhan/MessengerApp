using System;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Repository.Abstraction;
using MessengerApp.BLL.Services.Abstraction;
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

        public Task<Result<Pager<User>>> GetUsersPageAsync(string? search, int page, int items) 
            => _unitOfWork.Users.GetUsersPageAsync(search, page, items);

        public Task<Result<User>> GetUserAsync(int id) 
            => _unitOfWork.Users.GetUserAsync(id);

        public async Task<Result<User>> EditUserAsync(EditUserDto editUserDto)
        {
            try
            {
                var editUserResult = await _unitOfWork.Users.EditUserAsync(editUserDto);

                if (!editUserResult.Success)
                    return editUserResult;

                var userEntity = editUserResult.Data;
                
                var removePassword = await _userManager.RemovePasswordAsync(userEntity);

                if(!removePassword.Succeeded)
                    return Result<User>.CreateFailed(AccountResultConstants.ErrorRemovingPassword);
                
                var addPass = await _userManager.AddPasswordAsync(userEntity, editUserDto.NewPassword);

                return !addPass.Succeeded
                    ? Result<User>.CreateFailed(AccountResultConstants.ErrorAddingPassword)
                    : Result<User>.CreateSuccess(userEntity);
            }
            catch (Exception e)
            {
                return Result<User>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public Task<Result> DeleteUserAsync(int id) 
            => _unitOfWork.Users.DeleteUserAsync(id);
    }
}