using System;
using System.Threading.Tasks;
using MessengerApp.BLL.Services;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO.Authorization;
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
        [InlineData("newUser@gmail.com", 25, "newAccess", 1)]
        public async Task EditUserAsync_EditUserDto_SuccessEditedUserReturned(
            string newEmail, int newAge, string newPassword, int id)
        {
            var editUserDto = new EditUserDto(newEmail, newAge, newPassword, id);

            var userEntity = new User
            {
                Email = newEmail,
                Age = newAge
            };

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.Users
                .EditUserAsync(editUserDto)
            ).Returns(Task.FromResult(Result<User>.CreateSuccess(userEntity)));

            _userManagerMock.Setup(userManager => userManager
                .RemovePasswordAsync(userEntity)
            ).Returns(Task.FromResult(IdentityResult.Success));

            _userManagerMock.Setup(userManager => userManager
                .AddPasswordAsync(userEntity, editUserDto.NewPassword)
            ).Returns(Task.FromResult(IdentityResult.Success));

            var actual = await _adminService.EditUserAsync(editUserDto);

            var expected = Result<User>.CreateSuccess(userEntity);

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.True(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }

        [Theory]
        [InlineData("newUser@gmail.com", 25, "newAccess", 1)]
        public async Task EditUserAsync_EditUserDto_FailFromRepositoryReturned(
            string newEmail, int newAge, string newPassword, int id)
        {
            var editUserDto = new EditUserDto(newEmail, newAge, newPassword, id);

            var userEntity = new User
            {
                Email = newEmail,
                Age = newAge
            };

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.Users
                .EditUserAsync(editUserDto)
            ).Returns(Task.FromResult(Result<User>.CreateFailed(
                AccountResultConstants.UserNotFound,
                new NullReferenceException()))
            );

            var actual = await _adminService.EditUserAsync(editUserDto);

            var expected = Result<User>.CreateFailed(
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
        [InlineData("newUser@gmail.com", 25, "newAccess", 1)]
        public async Task EditUserAsync_EditUserDto_FailRemovingPasswordReturned(
            string newEmail, int newAge, string newPassword, int id)
        {
            var editUserDto = new EditUserDto(newEmail, newAge, newPassword, id);

            var userEntity = new User
            {
                Email = newEmail,
                Age = newAge
            };

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.Users
                .EditUserAsync(editUserDto)
            ).Returns(Task.FromResult(Result<User>.CreateSuccess(userEntity)));

            _userManagerMock.Setup(userManager => userManager
                .RemovePasswordAsync(userEntity)
            ).Returns(Task.FromResult(IdentityResult.Failed()));

            var actual = await _adminService.EditUserAsync(editUserDto);

            var expected = Result<User>.CreateFailed(AccountResultConstants.ErrorRemovingPassword);

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }
        
        [Theory]
        [InlineData("newUser@gmail.com", 25, "newAccess", 1)]
        public async Task EditUserAsync_EditUserDto_FailAddingPasswordReturned(
            string newEmail, int newAge, string newPassword, int id)
        {
            var editUserDto = new EditUserDto(newEmail, newAge, newPassword, id);

            var userEntity = new User
            {
                Email = newEmail,
                Age = newAge
            };

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.Users
                .EditUserAsync(editUserDto)
            ).Returns(Task.FromResult(Result<User>.CreateSuccess(userEntity)));

            _userManagerMock.Setup(userManager => userManager
                .RemovePasswordAsync(userEntity)
            ).Returns(Task.FromResult(IdentityResult.Success));

            _userManagerMock.Setup(userManager => userManager
                .AddPasswordAsync(userEntity, editUserDto.NewPassword)
            ).Returns(Task.FromResult(IdentityResult.Failed()));

            var actual = await _adminService.EditUserAsync(editUserDto);

            var expected = Result<User>.CreateFailed(AccountResultConstants.ErrorAddingPassword);

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }
        
        [Theory]
        [InlineData("newUser@gmail.com", 25, "newAccess", 1)]
        public async Task EditUserAsync_EditUserDto_FailUnexpectedReturned(
            string newEmail, int newAge, string newPassword, int id)
        {
            var editUserDto = new EditUserDto(newEmail, newAge, newPassword, id);

            var userEntity = new User
            {
                Email = newEmail,
                Age = newAge
            };

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.Users
                .EditUserAsync(editUserDto)
            ).Throws(new Exception());

            var actual = await _adminService.EditUserAsync(editUserDto);

            var expected = Result<User>.CreateFailed(CommonResultConstants.Unexpected, new Exception());

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }
    }
}