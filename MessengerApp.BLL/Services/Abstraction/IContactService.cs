using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Contact;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;

namespace MessengerApp.BLL.Services.Abstraction
{
    public interface IContactService
    {
        Task<Result<Pager<ContactDto>>> GetContactsPageAsync(
            int userId, string? search, int page, int items);

        Task<Result<ContactDto>> GetContactAsync(
            int userId, int userContactId);

        Task<Result<ContactDto>> CreateContactAsync(
            int userId, CreateContactDto createContactDto);

        Task<Result<ContactDto>> EditContactAsync(
            int userId, EditContactDto editContactDto);

        Task<Result> DeleteContactAsync(
            int userId, int contactId);
    }
}