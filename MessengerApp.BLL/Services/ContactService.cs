using System.Threading.Tasks;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Contact;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Repository.Abstraction;

namespace MessengerApp.BLL.Services
{
    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Result<Pager<ContactDto>>> GetContactsPageAsync(
            int userId, string? search, int page, int items
        ) =>
            _unitOfWork.Contact.GetContactsPageAsync(userId, search, page, items);

        public Task<Result<ContactDto>> GetContactAsync(
            int userId, int userContactId
        ) =>
            _unitOfWork.Contact.GetContactAsync(userId, userContactId);

        public Task<Result<ContactDto>> CreateContactAsync(
            int userId, CreateContactDto createContactDto
        ) =>
            _unitOfWork.Contact.CreateContactAsync(userId, createContactDto);

        public Task<Result<ContactDto>> EditContactAsync(
            int userId, EditContactDto editContactDto
        ) =>
            _unitOfWork.Contact.EditContactAsync(userId, editContactDto);

        public Task<Result> DeleteContactAsync(
            int userId, int contactId
        ) =>
            _unitOfWork.Contact.DeleteContactAsync(userId, contactId);
    }
}