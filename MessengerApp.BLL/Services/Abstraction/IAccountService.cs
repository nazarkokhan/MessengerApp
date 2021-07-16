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
        Task<Result<Pager<UserDto>>> GetUsersInChatAsync(
            int chatId, string? search, int page, int items);
        
        Task<Result> CreateUserAndSendEmailTokenAsync(
            RegisterDto register);

        Task<Result> ConfirmRegistrationWithTokenAsync(
            string token, string userId);
        
        Task<Result<Token>> GetAccessTokenAsync(
            LogInUserDto userInput);

        Task<Result<ProfileDto>> GetProfile(
            int userId);

        Task<Result> SendEmailResetTokenAsync(
            ResetEmailDto resetEmailDto, int userId);

        Task<Result> ResetEmailAsync(
            string token, string newEmail, int userId);

        Task<Result> SendPasswordResetTokenAsync(
            ResetPasswordDto resetPasswordDto);

        Task<Result> ResetPasswordAsync(
            TokenPasswordDto tokenPasswordDto);
    }
}