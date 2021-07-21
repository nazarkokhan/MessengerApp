using System;
using System.Linq;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Contact;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.EF;
using MessengerApp.DAL.Entities;
using MessengerApp.DAL.Extensions;
using MessengerApp.DAL.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace MessengerApp.DAL.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly MsgContext _db;

        public ContactRepository(MsgContext context)
        {
            _db = context;
        }

        public async Task<Result<Pager<ContactDto>>> GetContactsPageAsync(
            int userId, string? search, int page, int items)
        {
            try
            {
                var contacts = await _db.Contacts
                    .Where(c => c.UserId == userId)
                    .OrderBy(c => c.UserId)
                    .Select(c => c.User)
                    .Select(u => u.MapUserContactDto())
                    .ToListAsync();

                if (!contacts.Any())
                    return Result<Pager<ContactDto>>.CreateFailed(
                        ContactResultConstants.ContactNotFount,
                        new NullReferenceException()
                    );

                return Result<Pager<ContactDto>>.CreateSuccess(
                    new Pager<ContactDto>(contacts, contacts.Count)
                );
            }
            catch (Exception e)
            {
                return Result<Pager<ContactDto>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<ContactDto>> GetContactAsync(
            int userId, int userContactId)
        {
            try
            {
                var contactEntity = await _db.Contacts.FindAsync(new Contact
                {
                    UserId = userId,
                    UserContactId = userContactId
                });

                if (contactEntity is null)
                    return Result<ContactDto>.CreateFailed(
                        ContactResultConstants.ContactNotFount,
                        new NullReferenceException()
                    );

                var contactDto = contactEntity.User.MapUserContactDto();

                return Result<ContactDto>.CreateSuccess(contactDto);
            }
            catch (Exception e)
            {
                return Result<ContactDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<ContactDto>> CreateContactAsync(
            int userId, CreateContactDto createContactDto)
        {
            try
            {
                var contact = createContactDto
                    .MapContact(userId);

                await _db.Contacts.AddAsync(contact);

                return await GetContactAsync(contact.UserId, contact.UserContactId);
            }
            catch (Exception e)
            {
                return Result<ContactDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<ContactDto>> EditContactAsync(
            int userId, EditContactDto editContactDto)
        {
            try
            {
                var contactEntity = await _db.Contacts
                    .FindAsync(
                        editContactDto.MapContact(userId)
                    );

                if (contactEntity is null)
                    return Result<ContactDto>.CreateFailed(ContactResultConstants.ContactNotFount);

                contactEntity.MapEditContact(editContactDto);

                await _db.SaveChangesAsync();

                return await GetContactAsync(userId, editContactDto.NewUserContactId);
            }
            catch (Exception e)
            {
                return Result<ContactDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result> DeleteContactAsync(
            int userId, int contactId)
        {
            try
            {
                var contactEntity = await _db.Contacts
                    .FindAsync(
                        new Contact
                        {
                            UserId = userId,
                            UserContactId = contactId
                        }
                    );

                if (contactEntity is null)
                    return Result.CreateFailed(ContactResultConstants.ContactNotFount, new NullReferenceException());

                _db.Contacts.Remove(contactEntity);

                return Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
    }
}