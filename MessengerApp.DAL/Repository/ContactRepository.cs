using System;
using System.Linq;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.User;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.EF;
using MessengerApp.DAL.Entities;
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

        public async Task<Result<Pager<UserContactDto>>> GetUserContactsPageAsync(
            int userId, string? search, int page, int items)
        {
            try
            {
                var contacts = await _db.Contacts
                    .Where(c => c.UserId == userId)
                    .Select(c => c.User)
                    .Select(u => u.MapUserContactDto())
                    .ToListAsync();

                if (!contacts.Any())
                    return Result<Pager<UserContactDto>>.CreateFailed(
                        ContactResultConstants.ContactNotFount,
                        new NullReferenceException()
                    );

                return Result<Pager<UserContactDto>>.CreateSuccess(
                    new Pager<UserContactDto>(contacts, contacts.Count)
                );
            }
            catch (Exception e)
            {
                return Result<Pager<UserContactDto>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<UserContactDto>> GetUserContactAsync(
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
                    return Result<UserContactDto>.CreateFailed(
                        ContactResultConstants.ContactNotFount,
                        new NullReferenceException()
                    );

                var contactDto = contactEntity.User.MapUserContactDto();

                return Result<UserContactDto>.CreateSuccess(contactDto);
            }
            catch (Exception e)
            {
                return Result<UserContactDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<UserContactDto>> AddUserContactAsync(
            int userId, int userContactId)
        {
            try
            {
                var contact = new Contact
                {
                    UserId = userId,
                    UserContactId = userContactId
                };
                await _db.Contacts.AddAsync(contact);

                return await GetUserContactAsync(contact.UserId, contact.UserContactId);
            }
            catch (Exception e)
            {
                return Result<UserContactDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<UserContactDto>> EditUserContactAsync(
            int userId, int userContactId, int newUserContactId)
        {
            try
            {
                var removeContact = await RemoveUserFromContactsAsync(userId, userContactId);

                if (!removeContact.Success)
                    return Result<UserContactDto>.CreateFailed(removeContact.Messages, removeContact.Exception);
                
                var addContact = await AddUserContactAsync(userId, newUserContactId);
                
                if (!addContact.Success)
                    return addContact;

                return await GetUserContactAsync(userId, newUserContactId);
            }
            catch (Exception e)
            {
                return Result<UserContactDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result> RemoveUserFromContactsAsync(
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