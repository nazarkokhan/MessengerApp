using System.Threading.Tasks;

namespace MessengerApp.BLL.Services.Abstraction
{
    public interface IEmailService
    {
        Task SendAsync(
            string to, string body, string? subject = null);
    }
}