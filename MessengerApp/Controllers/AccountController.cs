using System.Threading.Tasks;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.DTO.Authorization.Reset;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register/token")]
        public async Task<IActionResult> CreteUserAndSendEmailToken(RegisterDto register) 
            => (await _accountService.CreateUserAndSendEmailTokenAsync(register)).ToActionResult();

        [HttpGet("register")]
        public async Task<IActionResult> ConfirmRegistration(string token, string userId) 
            => (await _accountService.ConfirmRegistrationAsync(token, userId)).ToActionResult();

        [HttpPost("login")]
        public async Task<IActionResult> LogInAsync(LogInUserDto userDto) 
            => (await _accountService.GetAccessTokenAsync(userDto)).ToActionResult();

        [BearerAuthorize(Roles.Admin | Roles.User)]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile() 
            => (await _accountService.GetProfile(User.GetUserId())).ToActionResult();

        [BearerAuthorize(Roles.Admin | Roles.User)]
        [HttpPut("reset-email/email-token")]
        public async Task<IActionResult> SendEmailResetTokenAsync(ResetEmailDto resetEmailDto) 
            => (await _accountService.SendEmailResetTokenAsync(resetEmailDto, User.GetUserId())).ToActionResult();

        [BearerAuthorize(Roles.Admin | Roles.User)]
        [HttpGet("reset-email")]
        public async Task<IActionResult> ResetEmailAsync(string token, string newEmail) 
            => (await _accountService.ResetEmailAsync(token, newEmail, User.GetUserId())).ToActionResult();

        [HttpPut("reset-password/email-token")]
        public async Task<IActionResult> SendPasswordResetTokenAsync(ResetPasswordDto resetPasswordDto) 
            => (await _accountService.SendPasswordResetTokenAsync(resetPasswordDto)).ToActionResult();

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(TokenPasswordDto tokenPasswordDto) 
            => (await _accountService.ResetPasswordAsync(tokenPasswordDto)).ToActionResult();
    }
}