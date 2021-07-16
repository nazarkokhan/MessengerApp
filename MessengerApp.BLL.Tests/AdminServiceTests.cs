﻿using System;
using System.Threading.Tasks;
using MessengerApp.BLL.Services;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO.User;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Repository.Abstraction;
using MessengerApp.DAL.Entities.Authorization;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace MessengerApp.BLL.Tests
{
    public class AdminServiceTests
    {
        private readonly IAdminService _adminService;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IUserStore<User>> _userStoreMock = new();

        public AdminServiceTests()
        {
            _userManagerMock =
                new Mock<UserManager<User>>(_userStoreMock.Object, null, null, null, null, null, null, null, null);

            _adminService =
                new AdminService(_userManagerMock.Object, _unitOfWorkMock.Object);
        }

        [Theory]
        [InlineData("newUser", "newUser@gmail.com", "newAccess", "about", 1)]
        public async Task EditUserAsync_EditUserDto_SuccessEditedUserReturned(
            string newUserName, string newEmail, string newPassword, string about, int id)
        {
            var editUserDto = new EditUserDto(newUserName, newEmail, newPassword, about, id);

            var userEntity = new User
            {
                Email = newEmail,
                UserName = newUserName
            };

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.Users
                .EditUserAsync(editUserDto)
            ).Returns(Task.FromResult(Result<UserDto>.CreateSuccess(userEntity.MapUserDto())));

            _userManagerMock.Setup(userManager => userManager
                .RemovePasswordAsync(userEntity)
            ).Returns(Task.FromResult(IdentityResult.Success));

            _userManagerMock.Setup(userManager => userManager
                .AddPasswordAsync(userEntity, editUserDto.NewPassword)
            ).Returns(Task.FromResult(IdentityResult.Success));

            var actual = await _adminService.EditUserAsync(editUserDto);

            var expected = Result<UserDto>.CreateSuccess(userEntity.MapUserDto());

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.True(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }

        [Theory]
        [InlineData("newUser", "newUser@gmail.com", "newAccess", "about", 1)]
        public async Task EditUserAsync_EditUserDto_FailFromRepositoryReturned(
            string newUserName, string newEmail, string newPassword, string about, int id)
        {
            var editUserDto = new EditUserDto(newUserName, newEmail, newPassword, about, id);

            var userEntity = new User
            {
                Email = newEmail,
                UserName = newUserName
            };

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.Users
                .EditUserAsync(editUserDto)
            ).Returns(Task.FromResult(Result<UserDto>.CreateFailed(
                AccountResultConstants.UserNotFound,
                new NullReferenceException()))
            );

            var actual = await _adminService.EditUserAsync(editUserDto);

            var expected = Result<UserDto>.CreateFailed(
                AccountResultConstants.UserNotFound,
                new NullReferenceException()
            );

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }

        [Theory]
        [InlineData("newUser", "newUser@gmail.com", "newAccess", "about", 1)]
        public async Task EditUserAsync_EditUserDto_FailRemovingPasswordReturned(
            string newUserName, string newEmail, string newPassword, string about, int id)
        {
            var editUserDto = new EditUserDto(newUserName, newEmail, newPassword, about, id);

            var userEntity = new User
            {
                Email = newEmail,
                UserName = newUserName
            };

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.Users
                .EditUserAsync(editUserDto)
            ).Returns(Task.FromResult(Result<UserDto>.CreateSuccess(userEntity.MapUserDto())));

            _userManagerMock.Setup(userManager => userManager
                .RemovePasswordAsync(userEntity)
            ).Returns(Task.FromResult(IdentityResult.Failed()));

            var actual = await _adminService.EditUserAsync(editUserDto);

            var expected = Result<UserDto>.CreateFailed(AccountResultConstants.ErrorRemovingPassword);

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }

        [Theory]
        [InlineData("newUser", "newUser@gmail.com", "newAccess", "about", 1)]
        public async Task EditUserAsync_EditUserDto_FailAddingPasswordReturned(
            string newUserName, string newEmail, string newPassword, string about, int id)
        {
            var editUserDto = new EditUserDto(newUserName, newEmail, newPassword, about, id);

            var userEntity = new User
            {
                Email = newEmail,
                UserName = newUserName
            };

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.Users
                .EditUserAsync(editUserDto)
            ).Returns(Task.FromResult(Result<UserDto>.CreateSuccess(userEntity.MapUserDto())));

            _userManagerMock.Setup(userManager => userManager
                .RemovePasswordAsync(userEntity)
            ).Returns(Task.FromResult(IdentityResult.Success));

            _userManagerMock.Setup(userManager => userManager
                .AddPasswordAsync(userEntity, editUserDto.NewPassword)
            ).Returns(Task.FromResult(IdentityResult.Failed()));

            var actual = await _adminService.EditUserAsync(editUserDto);

            var expected = Result<UserDto>.CreateFailed(AccountResultConstants.ErrorAddingPassword);

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }

        [Theory]
        [InlineData("newUser", "newUser@gmail.com", "newAccess", "about", 1)]
        public async Task EditUserAsync_EditUserDto_FailUnexpectedReturned(
            string newUserName, string newEmail, string newPassword, string about, int id)
        {
            var editUserDto = new EditUserDto(newUserName, newEmail, newPassword, about, id);

            var userEntity = new User
            {
                Email = newEmail,
                UserName = newUserName
            };

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.Users
                .EditUserAsync(editUserDto)
            ).Throws(new Exception());

            var actual = await _adminService.EditUserAsync(editUserDto);

            var expected = Result<UserDto>.CreateFailed(CommonResultConstants.Unexpected, new Exception());

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }
    }
}