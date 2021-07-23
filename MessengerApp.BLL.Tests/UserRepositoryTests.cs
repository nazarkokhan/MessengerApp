using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.DTO.User;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.EF;
using MessengerApp.DAL.Repository;
using MessengerApp.DAL.Repository.Abstraction;
using MessengerApp.DAL.Entities.Authorization;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace MessengerApp.BLL.Tests
{
    public class UserRepositoryTests
    {
        private readonly IUserRepository _userRepository;

        public UserRepositoryTests()
        {
            var dbSeeds = new List<User>
            {
                new() {Email = "admin@gmail.com"},
                new() {Email = "user@gmail.com"},
                new() {Email = "use2@gmail.com" },
                new() {Email = "user3@gmail.com" },
                new() {Email = "user4@gmail.com"},
                new() {Email = "user5@gmail.com"},
                new() {Email = "user6@gmail.com"},
                new() {Email = "user7@gmail.com"},
                new() {Email = "user8@gmail.com"}
            };

            var dbContextOptions = new DbContextOptionsBuilder<MsgContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            var db = new MsgContext(dbContextOptions);

            db.Users.AddRangeAsync(dbSeeds).GetAwaiter().GetResult();
            
            db.SaveChangesAsync().GetAwaiter().GetResult();

            _userRepository = new UserRepository(db);
        }

        [Theory]
        [InlineData("user")]
        [InlineData("admin", 1, 3)]
        [InlineData("user", 2, 1)]
        public async Task GetUsersPageAsync_SearchAndPageAndItems_SuccessPageOfUsersReturned(
            string search, int page = 1, int items = 5)
        {
            var actual = await _userRepository.GetUsersPageAsync(search, page, items);

            var expected = Result<Pager<User>>.CreateSuccess(It.IsAny<Pager<User>>());

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.True(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetUserAsync_Id_SuccessUserReturned(int id)
        {
            var actual = await _userRepository.GetUserAsync(id);

            var expected = Result<UserDto>.CreateSuccess(actual.Data);

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.True(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }

        [Theory]
        [InlineData(8393)]
        [InlineData(9991)]
        [InlineData(1932)]
        public async Task GetUserAsync_Id_FailUserNotFoundReturned(int id)
        {
            var actual = await _userRepository.GetUserAsync(id);

            var expected = Result<User>.CreateFailed(
                UserResultConstants.UserNotFound,
                new NullReferenceException()
            );

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
        }

        [Theory]
        [InlineData(9878, "admin", "admin@gmail.com", "adminAccess", "about")]
        [InlineData(4123, "user", "user@gmail.com", "userAccess", "about")]
        [InlineData(1, "newUser", "newUser@gmail.com", "newAccess", "about")]
        public async Task EditUserAsync_EditUserDto_SuccessEditedUserReturned(
            int id, string newUserName, string newEmail, string newPassword, string about)
        {
            var userDto = new EditUserByAdminDto(id, newUserName, newEmail, newPassword, about);

            var actual = await _userRepository.EditUserByAdminAsync(userDto);

            var expected = Result<UserDto>.CreateSuccess(actual.Data);

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.True(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }

        [Theory]
        [InlineData(9878, "admin", "admin@gmail.com", "adminAccess", "about")]
        [InlineData(4123, "user", "user@gmail.com", "userAccess", "about")]
        [InlineData(1, "newUser", "newUser@gmail.com", "newAccess", "about")]
        public async Task EditUserAsync_EditUserDto_FailUserNotFoundReturned(
            int id, string newUserName, string newEmail, string newPassword, string about)
        {
            var userDto = new EditUserByAdminDto(id, newUserName, newEmail, newPassword, about);

            var actual = await _userRepository.EditUserByAdminAsync(userDto);

            var expected = Result<User>.CreateFailed(
                UserResultConstants.UserNotFound,
                new NullReferenceException()
            );

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
        }
        
        [Theory]
        [InlineData("admin@gmail.com")]
        [InlineData("user@gmail.com")]
        [InlineData("user2@gmail.com")]
        public async Task UserExistsAsync_Email_FailUserNotFoundReturned(string email)
        {
            var actual = await _userRepository.UserExistsAsync(email);

            var expected = Result<bool>.CreateSuccess(true);

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.True(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }
        
        [Theory]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(9)]
        public async Task DeleteUserAsync_Id_SuccessReturned(int id)
        {
            var actual = await _userRepository.DeleteUserAsync(id);

            var expected = Result.CreateSuccess();

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.True(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
        }
        
        [Theory]
        [InlineData(5137)]
        [InlineData(7533)]
        [InlineData(9311)]
        public async Task DeleteUserAsync_Id_FailUserNotFoundReturned(int id)
        {
            var actual = await _userRepository.DeleteUserAsync(id);

            var expected = Result.CreateFailed(
                UserResultConstants.UserNotFound,
                new NullReferenceException()
            );

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
        }
    }
}