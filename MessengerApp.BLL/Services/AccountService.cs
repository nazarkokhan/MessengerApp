using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.DTO.Authorization.Reset;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Repository.Abstraction;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.User;
using MessengerApp.DAL.Entities.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MessengerApp.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IEmailService _emailService;

        private readonly UserManager<User> _userManager;

        private readonly IUnitOfWork _unitOfWork;

        private readonly ITokenService _tokenService;


        public AccountService(UserManager<User> userManager, IEmailService emailService, IUnitOfWork unitOfWork,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<Result> CreateUserAndSendEmailTokenAsync(RegisterDto register)
        {
            try
            {
                var userEntity = new User
                {
                    Email = register.Email,
                    UserName = register.UserName
                };

                if ((await _unitOfWork.Users.UserExistsAsync(register.Email)).Data)
                    return Result.CreateFailed(UserResultConstants.UserAlreadyExists);

                var createResult = await _userManager.CreateAsync(userEntity, register.Password);

                if (!createResult.Succeeded)
                    return Result.CreateFailed(UserResultConstants.ErrorCreatingUser);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(userEntity);

                var emailConfirmationToken =
                    HttpUtility.UrlEncode(token);

                var pureLink =
                    $"{AccountEmailServiceConstants.UseThisUrlToConfirmRegistration}" +
                    "https://localhost:5001/api/account/register/" +
                    $"{emailConfirmationToken}/" +
                    $"{userEntity.Id}";

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

        public async Task<Result<string>> ConfirmRegistrationWithTokenAsync(
            string token, string userId)
        {
            try
            {
                var decodedToken = token.Replace("%2f", "/");

                var userEntity = await _userManager.FindByIdAsync(userId);

                var tokenIsValid = await _userManager.ConfirmEmailAsync(userEntity, decodedToken);

                if (!tokenIsValid.Succeeded)
                    return Result<string>.CreateFailed(UserResultConstants.InvalidRegistrationToken);

                await _userManager.AddToRoleAsync(userEntity, Roles.User.ToString());

                await _emailService.SendAsync(
                    to: userEntity.Email,
                    body: "You have successfully created your account in library!",
                    subject: AccountEmailServiceConstants.RegistrationConfirmed
                );

                return Result<string>.CreateSuccess(AccountEmailServiceConstants.RegistrationConfirmed);
            }
            catch (Exception e)
            {
                return Result<string>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<TokenDto>> GetAccessAndRefreshTokensAsync(
            LogInUserDto userInput)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userInput.Email);

                if (user is null)
                    return Result<TokenDto>.CreateFailed(
                        UserResultConstants.UserNotFound,
                        new NullReferenceException()
                    );

                if (!await _userManager.CheckPasswordAsync(user, userInput.Password))
                    return Result<TokenDto>.CreateFailed(UserResultConstants.InvalidUserNameOrPassword);

                if (!user.EmailConfirmed)
                    return Result<TokenDto>.CreateFailed(UserResultConstants.UserEmailNotConfirmed);

                var userRole = await _userManager.GetRolesAsync(user);

                if (userRole is null)
                    return Result<TokenDto>.CreateFailed(
                        UserResultConstants.UserDoesntHaveRole,
                        new NullReferenceException()
                    );

                var tempToken = _tokenService.GenerateTempToken(user).Data;

                var refreshToken = _tokenService.GenerateRefreshToken(user).Data;

                return Result<TokenDto>.CreateSuccess(
                    new TokenDto(
                        tempToken.Token,
                        tempToken.ExpTime,
                        refreshToken.Token,
                        refreshToken.ExpTime
                    )
                );
            }
            catch (Exception e)
            {
                return Result<TokenDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<TokenDto>> RefreshAccessToken(
            RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var claims = new JwtSecurityTokenHandler()
                    .ValidateToken(refreshTokenDto.Token,
                        new TokenValidationParameters
                        {
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = AuthOptions.SymmetricSecurityKey,
                        },
                        out var securityToken
                    );

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg
                        .Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase)
                )
                    return Result<TokenDto>.CreateFailed(
                        UserResultConstants.InvalidRefreshToken,
                        new SecurityTokenException()
                    );

                var user = await _userManager.FindByIdAsync(claims.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                if (user is null)
                    return Result<TokenDto>.CreateFailed(UserResultConstants.UserNotFound,
                        new NullReferenceException());

                var tempToken = _tokenService.GenerateTempToken(user).Data;

                var validTo = DateTime.UnixEpoch.AddSeconds(
                    double.Parse(jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "exp")!.Value!)
                );
                
                return Result<TokenDto>.CreateSuccess(
                    new TokenDto(
                        tempToken.Token,
                        tempToken.ExpTime,
                        refreshTokenDto.Token,
                        validTo
                    )
                );
            }
            catch (Exception e)
            {
                return Result<TokenDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<ProfileDto>> GetProfile(
            int userId)
        {
            try
            {
                var userEntity = await _userManager.FindByIdAsync(userId.ToString());

                return Result<ProfileDto>.CreateSuccess(
                    new ProfileDto(
                        userEntity.Id,
                        userEntity.Email,
                        userEntity.UserName,
                        userEntity.About
                    )
                );
            }
            catch (Exception e)
            {
                return Result<ProfileDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public Task<Result<UserDto>> EditUserAsync(
            int id, EditUserDto editUserDto
        ) =>
            _unitOfWork.Users.EditUserAsync(id, editUserDto);

        public async Task<Result> SendEmailResetTokenAsync(
            int userId, ResetEmailDto resetEmailDto)
        {
            try
            {
                var userEntity = await _userManager
                    .FindByIdAsync(userId.ToString());

                if (userEntity is null)
                    return Result.CreateFailed(
                        UserResultConstants.UserNotFound,
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
                    $"/{changeEmailToken}" +
                    $"/{resetEmailDto.NewEmail}";

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
            int userId, string token, string newEmail)
        {
            try
            {
                var userEntity = await _userManager
                    .FindByIdAsync(userId.ToString());

                if (userEntity is null)
                    return Result.CreateFailed(
                        UserResultConstants.UserNotFound,
                        new NullReferenceException()
                    );

                var changeEmail = await _userManager
                    .ChangeEmailAsync(userEntity, newEmail, token);

                if (!changeEmail.Succeeded)
                    return Result.CreateFailed(UserResultConstants.InvalidResetEmailToken);

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
                        UserResultConstants.UserNotFound,
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
                        UserResultConstants.UserNotFound,
                        new NullReferenceException()
                    );

                var resetPassword = await _userManager.ResetPasswordAsync(
                    userEntity,
                    $"{tokenPasswordDto}",
                    tokenPasswordDto.NewPassword
                );

                return !resetPassword.Succeeded
                    ? Result.CreateFailed(UserResultConstants.InvalidResetPasswordToken)
                    : Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public Task<Result<Pager<UserDto>>> GetUsersInChatAsync(
            int userId, int chatId, string? search, int page, int items)
        {
            return _unitOfWork.Users.GetUsersInChatAsync(userId, chatId, search, page, items);
        }
    }
}