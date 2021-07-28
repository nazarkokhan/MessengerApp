using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.DTO.Authorization.Reset;
using MessengerApp.Core.DTO.User;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;

namespace MessengerApp.BLL.Services.Abstraction
{
    public interface IAccountService
    {
        Task<Result> CreateUserAndSendEmailTokenAsync(
            RegisterDto register);

        Task<Result<string>> ConfirmRegistrationWithTokenAsync(
            string token, string userId);
        
        Task<Result<TokenDto>> GetAccessAndRefreshTokensAsync(
            LogInUserDto userInput);

        Task<Result<AccessTokenDto>> RefreshAccessToken(
            RefreshTokenDto refreshTokenDto);

        Task<Result<ProfileDto>> GetProfile(
            int userId);

        Task<Result<UserDto>> EditUserAsync(
            int id, EditUserDto editUserDto);
        
        Task<Result> SendEmailResetTokenAsync(
            int userId, ResetEmailDto resetEmailDto);

        Task<Result> ResetEmailAsync(
            int userId, string token, string newEmail);

        Task<Result> SendPasswordResetTokenAsync(
            ResetPasswordDto resetPasswordDto);

        Task<Result> ResetPasswordAsync(
            TokenPasswordDto tokenPasswordDto);

        Task<Result<Pager<UserDto>>> GetUsersInChatAsync(
            int userId, int chatId, string? search, int page, int items);
    }
}