using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MessengerApp.BLL.Services.Abstraction;
using Microsoft.Extensions.Configuration;

namespace MessengerApp.BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtpClient;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpClient = new SmtpClient(_configuration["EmailSettings:Host"],
                int.Parse(_configuration["EmailSettings:Port"]))
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_configuration["EmailSettings:UserName"],
                    _configuration["EmailSettings:Password"])
            };
        }

        public Task SendAsync(
            string to, string body, string? subject) 
            => _smtpClient
                .SendMailAsync(
                    _configuration["EmailSettings:Email"],
                    to,
                    subject ?? string.Empty,
                    body
                );
    }
}