using System;
using System.Linq;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Contact;
using MessengerApp.Core.Extensions;
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
                var contactEntities = _db.Contacts
                    .Include(c => c.UserContact)
                    .OrderBy(c => c.UserId)
                    .Where(c => c.UserId == userId)
                    .Select(c => c.UserContact);

                if (!string.IsNullOrWhiteSpace(search))
                    contactEntities = contactEntities
                        .Where(c => c.UserName.Contains(search));

                if (!contactEntities.Any())
                    return Result<Pager<ContactDto>>.CreateFailed(
                        ContactResultConstants.ContactNotFount,
                        new NullReferenceException()
                    );

                return Result<Pager<ContactDto>>.CreateSuccess(
                    new Pager<ContactDto>(
                        await contactEntities
                            .TakePage(page, items)
                            .Select(u => u.MapUserContactDto())
                            .ToListAsync(),
                        await contactEntities.CountAsync()
                    )
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
                var conEntity = new Contact
                {
                    UserId = userId,
                    UserContactId = userContactId
                };

                var contactEntity = await _db.Contacts
                    .Include(c => c.UserContact)
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.UserContactId == userContactId);

                if (contactEntity is null)
                    return Result<ContactDto>.CreateFailed(
                        ContactResultConstants.ContactNotFount,
                        new NullReferenceException()
                    );

                return Result<ContactDto>.CreateSuccess(contactEntity.UserContact.MapUserContactDto());
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

                await _db.SaveChangesAsync();

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
                
                if(contactEntity.UserId != userId)
                    return Result<ContactDto>.CreateFailed(CommonResultConstants.NoRules);

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
                    return Result.CreateFailed(
                        ContactResultConstants.ContactNotFount,
                        new NullReferenceException()
                    );

                if (userId != contactEntity.UserId)
                    return Result.CreateFailed(CommonResultConstants.NoRules);

                _db.Contacts.Remove(contactEntity);

                await _db.SaveChangesAsync();

                return Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
    }
}