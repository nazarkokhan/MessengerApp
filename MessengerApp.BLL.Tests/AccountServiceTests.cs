using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessengerApp.BLL.Services;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.DTO.Authorization.Reset;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Entities;
using MessengerApp.DAL.Repository.Abstraction;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.DTO.Authorization.Reset;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Entities;
using MessengerApp.DAL.Repository.Abstraction;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace MessengerApp.BLL.Tests
{
    public class AccountServiceTests
    {
        private readonly IAccountService _accountService;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IUserStore<User>> _userStoreMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IEmailService> _emailServiceMock = new();

        public AccountServiceTests()
        {
            _userManagerMock =
                new Mock<UserManager<User>>(_userStoreMock.Object, null, null, null, null, null, null, null, null);

            _accountService =
                new AccountService(_userManagerMock.Object, _emailServiceMock.Object, _unitOfWorkMock.Object);
        }


        [Theory]
        [InlineData("user@gmail.com", "userAccess", 25)]
        [InlineData("user1@gmail.com", "user1Access", 20)]
        [InlineData("user2@gmail.com", "user2Access", 35)]
        public async Task CreateUserAndSendEmailToken_RegisterDto_SuccessReturned(
            string regEmail, string regPassword, int regAge)
        {
            var registerDto = new RegisterDto(regEmail, regPassword, regAge);

            var generatedToken = "smd=TOKEN_MOCK=12kd";

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork
                .Users.UserExistsAsync(It.IsAny<string>())
            ).Returns(Task.FromResult(Result<bool>.CreateSuccess(false)));

            _userManagerMock.Setup(userManager => userManager
                .CreateAsync(It.IsAny<User>(), It.IsAny<string>())
            ).Returns(Task.FromResult(IdentityResult.Success));

            _userManagerMock.Setup(usManager => usManager
                .GenerateEmailConfirmationTokenAsync(It.IsAny<User>())
            ).Returns(Task.FromResult(generatedToken));

            _emailServiceMock.Setup(emService => emService
                .SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
            ).Returns(Task.CompletedTask);

            var actual = await _accountService.CreateUserAndSendEmailTokenAsync(registerDto);

            var expected = Result.CreateSuccess();

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.True(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
        }

        [Theory]
        [InlineData("user@gmail.com", "userAccess", 25)]
        [InlineData("user1@gmail.com", "user1Access", 20)]
        [InlineData("user2@gmail.com", "user2Access", 35)]
        public async Task CreateUserAndSendEmailToken_RegisterDto_FailUserExistsReturned(
            string regEmail, string regPassword, int regAge)
        {
            var registerDto = new RegisterDto(regEmail, regPassword, regAge);

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork
                .Users.UserExistsAsync(It.IsAny<string>())
            ).Returns(Task.FromResult(Result<bool>.CreateSuccess(true)));

            var actual = await _accountService.CreateUserAndSendEmailTokenAsync(registerDto);

            var expected = Result.CreateFailed(AccountResultConstants.UserAlreadyExists);

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Messages, actual.Messages);
        }

        [Theory]
        [InlineData("user@gmail.com", "userAccess", 25)]
        [InlineData("user1@gmail.com", "user1Access", 20)]
        [InlineData("user2@gmail.com", "user2Access", 35)]
        public async Task CreateUserAndSendEmailToken_RegisterDto_FailCreatingUserReturned(
            string regEmail, string regPassword, int regAge)
        {
            var registerDto = new RegisterDto(regEmail, regPassword, regAge);

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork
                .Users.UserExistsAsync(It.IsAny<string>())
            ).Returns(Task.FromResult(Result<bool>.CreateSuccess(false)));

            _userManagerMock.Setup(userManager => userManager
                .CreateAsync(It.IsAny<User>(), It.IsAny<string>())
            ).Returns(Task.FromResult(IdentityResult.Failed()));

            var actual = await _accountService.CreateUserAndSendEmailTokenAsync(registerDto);

            var expected = Result.CreateFailed(AccountResultConstants.ErrorCreatingUser);

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Messages, actual.Messages);
        }

        [Theory]
        [InlineData("user@gmail.com", "userAccess", 25)]
        [InlineData("user1@gmail.com", "user1Access", 20)]
        [InlineData("user2@gmail.com", "user2Access", 35)]
        public async Task CreateUserAndSendEmailToken_RegisterDto_FailUnexpectedExceptionReturned(
            string regEmail, string regPassword, int regAge)
        {
            var registerDto = new RegisterDto(regEmail, regPassword, regAge);

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork
                .Users.UserExistsAsync(It.IsAny<string>())
            ).Throws(new Exception());

            var actual = await _accountService.CreateUserAndSendEmailTokenAsync(registerDto);

            var expected = Result.CreateFailed(CommonResultConstants.Unexpected, new Exception());

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Messages, actual.Messages);
        }

        [Theory]
        [InlineData("smd=TOKEN_MOCK=s1kd", "1")]
        [InlineData("smd=TOKEN_MOCK=s2kd", "2")]
        [InlineData("smd=TOKEN_MOCK=s3kd", "3")]
        public async Task ConfirmRegistrationAsync_TokenAndUserId_SuccessReturned(
            string token, string userId)
        {
            var userEntity = new User
            {
                Id = int.Parse(userId)
            };
            _userManagerMock.Setup(userManager => userManager
                .FindByIdAsync(userId)
            ).Returns(Task.FromResult(userEntity));

            _userManagerMock.Setup(userManager => userManager
                .ConfirmEmailAsync(userEntity, token)
            ).Returns(Task.FromResult(IdentityResult.Success));

            _userManagerMock.Setup(userManager => userManager
                .AddToRoleAsync(userEntity, Roles.User.ToString())
            ).Returns(Task.FromResult(IdentityResult.Success));

            _emailServiceMock.Setup(emService => emService
                .SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
            ).Returns(Task.CompletedTask);

            var actual = await _accountService.ConfirmRegistrationAsync(token, userId);

            var expected = Result.CreateSuccess();

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            // Assert.Null(actual.Messages);
            Assert.True(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
        }

        [Theory]
        [InlineData("smd=TOKEN_MOCK=s1kd", "1")]
        [InlineData("smd=TOKEN_MOCK=s2kd", "2")]
        [InlineData("smd=TOKEN_MOCK=s3kd", "3")]
        public async Task ConfirmRegistrationAsync_TokenAndUserId_FailConfirmingPasswordReturned(
            string token, string userId)
        {
            var userEntity = new User
            {
                Id = int.Parse(userId)
            };
            _userManagerMock.Setup(userManager => userManager
                .FindByIdAsync(userId)
            ).Returns(Task.FromResult(userEntity));

            _userManagerMock.Setup(userManager => userManager
                .ConfirmEmailAsync(userEntity, token)
            ).Returns(Task.FromResult(IdentityResult.Failed()));

            var actual = await _accountService.ConfirmRegistrationAsync(token, userId);

            var expected = Result.CreateFailed(AccountResultConstants.InvalidRegistrationToken);

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Messages, actual.Messages);
        }

        [Theory]
        [InlineData("smd=TOKEN_MOCK=s1kd", "1")]
        [InlineData("smd=TOKEN_MOCK=s2kd", "2")]
        [InlineData("smd=TOKEN_MOCK=s3kd", "3")]
        public async Task ConfirmRegistrationAsync_TokenAndUserId_FailUnexpectedExceptionReturned(
            string token, string userId)
        {
            var userEntity = new User
            {
                Id = int.Parse(userId)
            };
            _userManagerMock.Setup(userManager => userManager
                .FindByIdAsync(userId)
            ).Throws(new Exception());

            var actual = await _accountService.ConfirmRegistrationAsync(token, userId);

            var expected = Result.CreateFailed(CommonResultConstants.Unexpected, new Exception());

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Messages, actual.Messages);
        }

        [Theory]
        [InlineData("user@gmail.com", "userAccess")]
        [InlineData("user1@gmail.com", "user1Access")]
        [InlineData("user2@gmail.com", "user2Access")]
        public async Task GetAccessTokenAsync_LogInUserDto_SuccessReturned(
            string logInEmail, string logInPassword)
        {
            var userInput = new LogInUserDto(logInEmail, logInPassword);

            var userEntity = new User
            {
                Email = userInput.Email
            };

            _userManagerMock.Setup(userManager => userManager
                .FindByEmailAsync(userInput.Email)
            ).Returns(Task.FromResult(userEntity));

            _userManagerMock.Setup(userManager => userManager
                .CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())
            ).Returns(Task.FromResult(true));

            IList<string> roles = new List<string> {$"{Roles.User.ToString()}"};

            _userManagerMock.Setup(userManager => userManager
                .GetRolesAsync(userEntity)
            ).Returns(Task.FromResult(roles));

            var actual = await _accountService.GetAccessTokenAsync(userInput);

            var expected = Result<Token>.CreateSuccess(new Token(It.IsAny<string>()));

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            // Assert.Null(actual.Messages);
            Assert.True(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
        }

        [Theory]
        [InlineData("user@gmail.com", "userAccess")]
        [InlineData("user1@gmail.com", "user1Access")]
        [InlineData("user2@gmail.com", "user2Access")]
        public async Task GetAccessTokenAsync_LogInUserDto_FailUserNotFoundReturned(
            string logInEmail, string logInPassword)
        {
            var userInput = new LogInUserDto(logInEmail, logInPassword);

            var actual = await _accountService.GetAccessTokenAsync(userInput);

            var expected = Result<Token>.CreateFailed(
                AccountResultConstants.UserNotFound,
                new NullReferenceException()
            );

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Messages, actual.Messages);
        }

        [Theory]
        [InlineData("user@gmail.com", "userAccess")]
        [InlineData("user1@gmail.com", "user1Access")]
        [InlineData("user2@gmail.com", "user2Access")]
        public async Task GetAccessTokenAsync_LogInUserDto_FailCheckingPasswordReturned(
            string logInEmail, string logInPassword)
        {
            var userInput = new LogInUserDto(logInEmail, logInPassword);

            var userEntity = new User
            {
                Email = userInput.Email
            };

            _userManagerMock.Setup(userManager => userManager
                .FindByEmailAsync(userInput.Email)
            ).Returns(Task.FromResult(userEntity));

            _userManagerMock.Setup(userManager => userManager
                .CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())
            ).Returns(Task.FromResult(false));

            var actual = await _accountService.GetAccessTokenAsync(userInput);

            var expected = Result<Token>.CreateFailed(AccountResultConstants.InvalidUserNameOrPassword);

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Messages, actual.Messages);
        }

        [Theory]
        [InlineData("user@gmail.com", "userAccess")]
        [InlineData("user1@gmail.com", "user1Access")]
        [InlineData("user2@gmail.com", "user2Access")]
        public async Task GetAccessTokenAsync_LogInUserDto_FailUnexpectedExceptionReturned(
            string logInEmail, string logInPassword)
        {
            var userInput = new LogInUserDto(logInEmail, logInPassword);

            _userManagerMock.Setup(userManager => userManager
                .FindByEmailAsync(userInput.Email)
            ).Throws(new Exception());

            var actual = await _accountService.GetAccessTokenAsync(userInput);

            var expected = Result<Token>.CreateFailed(CommonResultConstants.Unexpected, new Exception());

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Messages, actual.Messages);
        }

        [Theory]
        [InlineData(1, "user@gmail.com")]
        [InlineData(2, "user1@gmail.com")]
        [InlineData(3, "user2@gmail.com")]
        public async Task GetProfile_IdFromClaims_SuccessReturned(int id, string email)
        {
            var expected = new ProfileDto(id, email);

            _userManagerMock
                .Setup(userManager => userManager
                    .FindByIdAsync(id.ToString())
                ).Returns(Task.FromResult(new User
                {
                    Id = id,
                    Email = email
                }));

            var actual = await _accountService.GetProfile(id);

            Assert.NotNull(actual);
            Assert.NotNull(actual.Data);
            Assert.True(actual.Success);
            Assert.Equal(expected.Id, actual.Data.Id);
            Assert.Equal(expected.Email, actual.Data.Email);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetProfile_IdFromClaims_FailReturned(int id)
        {
            _userManagerMock
                .Setup(userManager => userManager
                    .FindByIdAsync(id.ToString())
                ).Throws(new Exception());

            var actual = await _accountService.GetProfile(id);

            Assert.NotNull(actual);
            Assert.False(actual.Success);
        }

        [Theory]
        [InlineData("newuser@gmail.com", "newuser@gmail.com", 1)]
        [InlineData("newuser1@gmail.com", "newuser1@gmail.com", 2)]
        [InlineData("newuser2@gmail.com", "newuser2@gmail.com", 3)]
        public async Task SendEmailResetTokenAsync_ResetEmailDtoAndUserId_SuccessReturned(
            string newEmailDto, string confirmNewEmailDto, int userId)
        {
            var resetEmailDto = new ResetEmailDto(newEmailDto, confirmNewEmailDto);

            var userEntity = new User
            {
                Id = userId
            };

            _userManagerMock.Setup(userManager => userManager
                .FindByIdAsync(userId.ToString())
            ).Returns(Task.FromResult(userEntity));

            _userManagerMock.Setup(userManager => userManager
                .GenerateChangeEmailTokenAsync(userEntity, resetEmailDto.NewEmail)
            ).Returns(Task.FromResult(Guid.NewGuid().ToString()));

            _emailServiceMock.Setup(emService => emService
                .SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
            ).Returns(Task.CompletedTask);

            var actual = await _accountService.SendEmailResetTokenAsync(resetEmailDto, userId);

            var expected = Result.CreateSuccess();

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.True(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Empty(actual.Messages);
        }

        [Theory]
        [InlineData("newuser@gmail.com", "newuser@gmail.com", 1)]
        [InlineData("newuser1@gmail.com", "newuser1@gmail.com", 2)]
        [InlineData("newuser2@gmail.com", "newuser2@gmail.com", 3)]
        public async Task SendEmailResetTokenAsync_ResetEmailDtoAndUserId_FailUserNotFoundReturned(
            string newEmailDto, string confirmNewEmailDto, int userId)
        {
            var resetEmailDto = new ResetEmailDto(newEmailDto, confirmNewEmailDto);

            var actual = await _accountService.SendEmailResetTokenAsync(resetEmailDto, userId);

            var expected = Result.CreateFailed(
                AccountResultConstants.UserNotFound,
                new NullReferenceException()
            );

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Exception!.Message, actual.Exception.Message);
            Assert.Equal(expected.Success, actual.Success);
            Assert.NotNull(actual.Messages);
        }

        [Theory]
        [InlineData("newuser@gmail.com", "newuser@gmail.com", 1)]
        [InlineData("newuser1@gmail.com", "newuser1@gmail.com", 2)]
        [InlineData("newuser2@gmail.com", "newuser2@gmail.com", 3)]
        public async Task SendEmailResetTokenAsync_ResetEmailDtoAndUserId_FailUnexpectedExceptionReturned(
            string newEmailDto, string confirmNewEmailDto, int userId)
        {
            var resetEmailDto = new ResetEmailDto(newEmailDto, confirmNewEmailDto);

            _userManagerMock.Setup(userManager => userManager
                .FindByIdAsync(userId.ToString())
            ).Throws(new Exception());

            var actual = await _accountService.SendEmailResetTokenAsync(resetEmailDto, userId);

            var expected = Result.CreateFailed(CommonResultConstants.Unexpected, new Exception());

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
        }

        [Theory]
        [InlineData("smd=TOKEN_MOCK=s1kd", "newuser@gmail.com", 1)]
        [InlineData("smd=TOKEN_MOCK=s2kd", "newuser1@gmail.com", 2)]
        [InlineData("smd=TOKEN_MOCK=s3kd", "newuser2@gmail.com", 3)]
        public async Task ResetEmailAsync_TokenAndNewEmailAndUserId_SuccessReturned(
            string token, string newEmail, int userId)
        {
            var userEntity = new User
            {
                Id = userId
            };

            _userManagerMock.Setup(userManager => userManager
                .FindByIdAsync(userId.ToString())
            ).Returns(Task.FromResult(userEntity));

            _userManagerMock.Setup(userManager => userManager
                .ChangeEmailAsync(userEntity, newEmail, token)
            ).Returns(Task.FromResult(IdentityResult.Success));

            var actual = await _accountService.ResetEmailAsync(token, newEmail, userId);

            var expected = Result.CreateSuccess();

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.True(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
        }

        [Theory]
        [InlineData("smd=TOKEN_MOCK=s1kd", "newuser@gmail.com", 1)]
        [InlineData("smd=TOKEN_MOCK=s2kd", "newuser1@gmail.com", 2)]
        [InlineData("smd=TOKEN_MOCK=s3kd", "newuser2@gmail.com", 3)]
        public async Task ResetEmailAsync_TokenAndNewEmailAndUserId_FailUserNotFoundReturned(
            string token, string newEmail, int userId)
        {
            var actual = await _accountService.ResetEmailAsync(token, newEmail, userId);

            var expected = Result.CreateFailed(
                AccountResultConstants.UserNotFound,
                new NullReferenceException()
            );

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Exception!.Message, actual.Exception.Message);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Messages, actual.Messages);
        }

        [Theory]
        [InlineData("smd=TOKEN_MOCK=s1kd", "newuser@gmail.com", 1)]
        [InlineData("smd=TOKEN_MOCK=s2kd", "newuser1@gmail.com", 2)]
        [InlineData("smd=TOKEN_MOCK=s3kd", "newuser2@gmail.com", 3)]
        public async Task ResetEmailAsync_TokenAndNewEmailAndUserId_FailChangingEmailReturned(
            string token, string newEmail, int userId)
        {
            var userEntity = new User
            {
                Id = userId
            };

            _userManagerMock.Setup(userManager => userManager
                .FindByIdAsync(userId.ToString())
            ).Returns(Task.FromResult(userEntity));

            _userManagerMock.Setup(userManager => userManager
                .ChangeEmailAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())
            ).Returns(Task.FromResult(IdentityResult.Failed()));

            var actual = await _accountService.ResetEmailAsync(token, newEmail, userId);

            var expected = Result.CreateFailed(AccountResultConstants.InvalidResetEmailToken);

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Messages, actual.Messages);
        }

        [Theory]
        [InlineData("smd=TOKEN_MOCK=s1kd", "newuser@gmail.com", 1)]
        [InlineData("smd=TOKEN_MOCK=s2kd", "newuser1@gmail.com", 2)]
        [InlineData("smd=TOKEN_MOCK=s3kd", "newuser2@gmail.com", 3)]
        public async Task ResetEmailAsync_TokenAndNewEmailAndUserId_FailUnexpectedExceptionReturned(
            string token, string newEmail, int userId)
        {
            _userManagerMock.Setup(userManager => userManager
                .FindByIdAsync(userId.ToString())
            ).Throws(new Exception());

            var actual = await _accountService.ResetEmailAsync(token, newEmail, userId);

            var expected = Result.CreateFailed(CommonResultConstants.Unexpected, new Exception());

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
        }
        
        [Theory]
        [InlineData("user@gmail.com")]
        [InlineData("user1@gmail.com")]
        [InlineData("user2@gmail.com")]
        public async Task SendPasswordResetTokenAsync_ResetPasswordDto_SuccessReturned(string resEmail)
        {
            var resetPasswordDto = new ResetPasswordDto(resEmail);

            var userEntity = new User
            {
                Email = resetPasswordDto.Email
            };
            
            _userManagerMock.Setup(userManager => userManager
                .FindByEmailAsync(resetPasswordDto.Email)
            ).Returns(Task.FromResult(userEntity));

            _userManagerMock.Setup(userManager => userManager
                .GeneratePasswordResetTokenAsync(userEntity)
            ).Returns(Task.FromResult(Guid.NewGuid().ToString()));

            _emailServiceMock.Setup(emService => emService
                .SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
            ).Returns(Task.CompletedTask);

            var actual = await _accountService.SendPasswordResetTokenAsync(resetPasswordDto);

            var expected = Result.CreateSuccess();

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.True(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Empty(actual.Messages);
        }

        [Theory]
        [InlineData("user@gmail.com")]
        [InlineData("user1@gmail.com")]
        [InlineData("user2@gmail.com")]
        public async Task SendPasswordResetTokenAsync_ResetPasswordDto_FailUserNotFoundReturned(string resEmail)
        {
            var resetPasswordDto = new ResetPasswordDto(resEmail);

            var actual = await _accountService.SendPasswordResetTokenAsync(resetPasswordDto);

            var expected = Result.CreateFailed(
                AccountResultConstants.UserNotFound,
                new NullReferenceException()
            );

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Exception!.Message, actual.Exception.Message);
            Assert.Equal(expected.Success, actual.Success);
            Assert.NotNull(actual.Messages);
        }

        [Theory]
        [InlineData("user@gmail.com")]
        [InlineData("user1@gmail.com")]
        [InlineData("user2@gmail.com")]
        public async Task SendPasswordResetTokenAsync_ResetPasswordDto_FailUnexpectedExceptionReturned(string resEmail)
        {
            var resetPasswordDto = new ResetPasswordDto(resEmail);

            _userManagerMock.Setup(userManager => userManager
                .FindByEmailAsync(resetPasswordDto.Email)
            ).Throws(new Exception());

            var actual = await _accountService.SendPasswordResetTokenAsync(resetPasswordDto);

            var expected = Result.CreateFailed(CommonResultConstants.Unexpected, new Exception());

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
        }
        
        [Theory]
        [InlineData("user@gmail.com", "newAccess", "newAccess", "smd=TOKEN_MOCK=s1kd")]
        [InlineData("user1@gmail.com", "new1Access", "new1Access", "smd=TOKEN_MOCK=s2kd")]
        [InlineData("user2@gmail.com", "new2Access", "new2Access", "smd=TOKEN_MOCK=s3kd")]
        public async Task ResetPasswordAsync_TokenPasswordDto_SuccessReturned(
            string email, string newPassword, string confirmNewPassword, string token)
        {
            var tokenPasswordDto = new TokenPasswordDto
            (
                email,
                newPassword,
                confirmNewPassword,
                token
            );
            
            var userEntity = new User
            {
                Email = tokenPasswordDto.Email
            };

            _userManagerMock.Setup(userManager => userManager
                .FindByEmailAsync(tokenPasswordDto.Email)
            ).Returns(Task.FromResult(userEntity));

            _userManagerMock.Setup(userManager => userManager
                .ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())
            ).Returns(Task.FromResult(IdentityResult.Success));

            var actual = await _accountService.ResetPasswordAsync(tokenPasswordDto);

            var expected = Result.CreateSuccess();

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.True(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
        }

        [Theory]
        [InlineData("user@gmail.com", "newAccess", "newAccess", "smd=TOKEN_MOCK=s1kd")]
        [InlineData("user1@gmail.com", "new1Access", "new1Access", "smd=TOKEN_MOCK=s2kd")]
        [InlineData("user2@gmail.com", "new2Access", "new2Access", "smd=TOKEN_MOCK=s3kd")]
        public async Task ResetPasswordAsync_TokenPasswordDto_FailUserNotFoundReturned(
            string email, string newPassword, string confirmNewPassword, string token)
        {
            var tokenPasswordDto = new TokenPasswordDto
            (
                email,
                newPassword,
                confirmNewPassword,
                token
            );
            
            var actual = await _accountService.ResetPasswordAsync(tokenPasswordDto);

            var expected = Result.CreateFailed(
                AccountResultConstants.UserNotFound,
                new NullReferenceException()
            );

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Exception!.Message, actual.Exception.Message);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Messages, actual.Messages);
        }

        [Theory]
        [InlineData("user@gmail.com", "newAccess", "newAccess", "smd=TOKEN_MOCK=s1kd")]
        [InlineData("user1@gmail.com", "new1Access", "new1Access", "smd=TOKEN_MOCK=s2kd")]
        [InlineData("user2@gmail.com", "new2Access", "new2Access", "smd=TOKEN_MOCK=s3kd")]
        public async Task ResetPasswordAsync_TokenPasswordDto_FailChangingPasswordReturned(
            string email, string newPassword, string confirmNewPassword, string token)
        {
            var tokenPasswordDto = new TokenPasswordDto
            (
                email,
                newPassword,
                confirmNewPassword,
                token
            );

            var userEntity = new User
            {
                Email = tokenPasswordDto.Email
            };

            _userManagerMock.Setup(userManager => userManager
                .FindByEmailAsync(tokenPasswordDto.Email)
            ).Returns(Task.FromResult(userEntity));

            _userManagerMock.Setup(userManager => userManager
                .ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())
            ).Returns(Task.FromResult(IdentityResult.Failed()));

            var actual = await _accountService.ResetPasswordAsync(tokenPasswordDto);

            var expected = Result.CreateFailed(AccountResultConstants.InvalidResetPasswordToken);

            Assert.NotNull(actual);
            Assert.Null(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
            Assert.Equal(expected.Messages, actual.Messages);
        }

        [Theory]
        [InlineData("user@gmail.com", "newAccess", "newAccess", "smd=TOKEN_MOCK=s1kd")]
        [InlineData("user1@gmail.com", "new1Access", "new1Access", "smd=TOKEN_MOCK=s2kd")]
        [InlineData("user2@gmail.com", "new2Access", "new2Access", "smd=TOKEN_MOCK=s3kd")]
        public async Task ResetPasswordAsync_TokenPasswordDto_FailUnexpectedExceptionReturned(
            string email, string newPassword, string confirmNewPassword, string token)
        {
            var tokenPasswordDto = new TokenPasswordDto
            (
                email,
                newPassword,
                confirmNewPassword,
                token
            );

            _userManagerMock.Setup(userManager => userManager
                .FindByEmailAsync(tokenPasswordDto.Email)
            ).Throws(new Exception());

            var actual = await _accountService.ResetPasswordAsync(tokenPasswordDto);

            var expected = Result.CreateFailed(CommonResultConstants.Unexpected, new Exception());

            Assert.NotNull(actual);
            Assert.NotNull(actual.Exception);
            Assert.False(actual.Success);
            Assert.Equal(expected.Success, actual.Success);
        }
    }
}