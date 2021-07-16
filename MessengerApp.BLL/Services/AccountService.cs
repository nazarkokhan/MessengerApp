using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using MessengerApp.Core;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.DTO.Authorization.Reset;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Repository.Abstraction;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.User;
using MessengerApp.DAL.Entities.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MessengerApp.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IEmailService _emailService;

        private readonly UserManager<User> _userManager;

        private readonly IUnitOfWork _unitOfWork;

        public AccountService(UserManager<User> userManager, IEmailService emailService, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public Task<Result<Pager<UserDto>>> GetUsersInChatAsync(
            int chatId, string? search, int page, int items)
        {
            return _unitOfWork.Users.GetUsersInChatAsync(chatId, search, page, items);
        }
        public async Task<Result> CreateUserAndSendEmailTokenAsync(RegisterDto register)
        {
            try
            {
                var userEntity = new User
                {
                    Email = register.Email,
                    UserName = register.UserName,
                };

                if ((await _unitOfWork.Users.UserExistsAsync(register.Email)).Data)
                    return Result.CreateFailed(AccountResultConstants.UserAlreadyExists);

                var createResult = await _userManager.CreateAsync(userEntity, register.Password);

                if (!createResult.Succeeded)
                    return Result.CreateFailed(AccountResultConstants.ErrorCreatingUser);

                var emailConfirmationToken =
                    HttpUtility.UrlEncode(await _userManager.GenerateEmailConfirmationTokenAsync(userEntity));

                var pureLink =
                    $"https://localhost:5001/api/account/register?token={emailConfirmationToken}&userId={userEntity.Id}";

                // var htmlLink =
                //     "<a class=\"link\" href=\"https://localhost:5001/api/account/register?" +
                //     $"token={emailConfirmationToken}&userId={userEntity.Id}\">Confirm registration</a>\n";

                await _emailService.SendAsync(
                    to: userEntity.Email,
                    body: pureLink,
                    subject: AccountEmailServiceConstants.ConfirmRegistration
                );

                return Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result> ConfirmRegistrationWithTokenAsync(
            string token, string userId)
        {
            try
            {
                var userEntity = await _userManager.FindByIdAsync(userId);

                var tokenIsValid = await _userManager.ConfirmEmailAsync(userEntity, token);

                if (!tokenIsValid.Succeeded)
                    return Result.CreateFailed(AccountResultConstants.InvalidRegistrationToken);

                await _userManager.AddToRoleAsync(userEntity, Roles.User.ToString());

                await _emailService.SendAsync(
                    to: userEntity.Email,
                    body: "You have successfully created your account in library!",
                    subject: AccountEmailServiceConstants.RegistrationConfirmed
                );

                return Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<Token>> GetAccessTokenAsync(
            LogInUserDto userInput)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userInput.Email);

                if (user is null)
                    return Result<Token>.CreateFailed(
                        AccountResultConstants.UserNotFound,
                        new NullReferenceException()
                    );

                if (!await _userManager.CheckPasswordAsync(user, userInput.Password))
                    return Result<Token>.CreateFailed(AccountResultConstants.InvalidUserNameOrPassword);

                var timeNow = DateTime.Now;

                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.Issuer,
                    audience: AuthOptions.Audience,
                    notBefore: timeNow,
                    claims: new List<Claim>
                    {
                        new(ClaimTypes.Email, user.Email),
                        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new(ClaimTypes.Role, (await _userManager.GetRolesAsync(user)).FirstOrDefault()!)
                    },
                    expires: timeNow.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
                    signingCredentials: new SigningCredentials(AuthOptions.SymmetricSecurityKey,
                        SecurityAlgorithms.HmacSha256)
                );

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                return Result<Token>.CreateSuccess(new Token(encodedJwt));
            }
            catch (Exception e)
            {
                return Result<Token>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<ProfileDto>> GetProfile(
            int userId)
        {
            try
            {
                var userEntity = await _userManager.FindByIdAsync(userId.ToString());

                return Result<ProfileDto>.CreateSuccess(
                    new ProfileDto
                    (
                        userEntity.Id,
                        userEntity.Email
                    )
                );
            }
            catch (Exception e)
            {
                return Result<ProfileDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result> SendEmailResetTokenAsync(
            ResetEmailDto resetEmailDto, int userId)
        {
            try
            {
                var userEntity = await _userManager
                    .FindByIdAsync(userId.ToString());

                if (userEntity is null)
                    return Result.CreateFailed(
                        AccountResultConstants.UserNotFound,
                        new NullReferenceException()
                    );

                var changeEmailToken = HttpUtility.UrlEncode(await _userManager
                    .GenerateChangeEmailTokenAsync(
                        userEntity,
                        resetEmailDto.NewEmail
                    )
                );

                var pureLink =
                    "https://localhost:5001/api/account/reset-email" +
                    $"?token={changeEmailToken}" +
                    $"&newEmail={resetEmailDto.NewEmail}";

                await _emailService
                    .SendAsync(
                        userEntity.Email,
                        pureLink,
                        AccountEmailServiceConstants.ConfirmEmailReset
                    );

                return Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result> ResetEmailAsync(
            string token, string newEmail, int userId)
        {
            try
            {
                var userEntity = await _userManager
                    .FindByIdAsync(userId.ToString());

                if (userEntity is null)
                    return Result.CreateFailed(
                        AccountResultConstants.UserNotFound,
                        new NullReferenceException()
                    );

                var changeEmail = await _userManager
                    .ChangeEmailAsync(userEntity, newEmail, token);

                if (!changeEmail.Succeeded)
                    return Result.CreateFailed(AccountResultConstants.InvalidResetEmailToken);

                userEntity.UserName = newEmail;

                return Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result> SendPasswordResetTokenAsync(
            ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var userEntity = await _userManager
                    .FindByEmailAsync(resetPasswordDto.Email);

                if (userEntity is null)
                    return Result.CreateFailed(
                        AccountResultConstants.UserNotFound,
                        new NullReferenceException()
                    );

                var passwordResetToken = await _userManager
                    .GeneratePasswordResetTokenAsync(userEntity);

                await _emailService.SendAsync(
                    resetPasswordDto.Email,
                    passwordResetToken,
                    AccountEmailServiceConstants.ConfirmPasswordReset
                );

                return Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result> ResetPasswordAsync(
            TokenPasswordDto tokenPasswordDto)
        {
            try
            {
                var userEntity = await _userManager
                    .FindByEmailAsync(tokenPasswordDto.Email);

                if (userEntity is null)
                    return Result.CreateFailed(
                        AccountResultConstants.UserNotFound,
                        new NullReferenceException()
                    );

                var resetPassword = await _userManager.ResetPasswordAsync(
                    userEntity,
                    $"{tokenPasswordDto}",
                    tokenPasswordDto.NewPassword
                );

                return !resetPassword.Succeeded
                    ? Result.CreateFailed(AccountResultConstants.InvalidResetPasswordToken)
                    : Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
    }
}