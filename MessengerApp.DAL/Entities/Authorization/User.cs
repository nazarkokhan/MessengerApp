using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using MessengerApp.Core.DTO.Contact;
using MessengerApp.Core.DTO.User;
using MessengerApp.DAL.Entities.Abstract;
using Microsoft.AspNetCore.Identity;

// ReSharper disable All
#pragma warning disable 8618

namespace MessengerApp.DAL.Entities.Authorization
{
    public class User : IdentityUser<int>, IEntity<int>
    {
        // public override int Id { get; set; }
        
        [Required]
        [MaxLength(10)]
        public override string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public override string Email { get; set; }
        
        [MaxLength(512)]
        public string About { get; set; }
        
        public ICollection<Contact> UserContacts { get; set; }
        
        public ICollection<Contact> ContactUsers { get; set; }

        public ICollection<Message> Messages { get; set; }

        [DataMember]
        public ICollection<ChatUser> ChatUsers { get; set; }

        public ContactDto MapUserContactDto()
        {
            return new ContactDto(UserName, About, Email);
        }

        public UserDto MapUserDto()
        {
            return new UserDto
            (
                UserName,
                About,
                Email,
                UserContacts.Select(c => c.Id).ToList(),
                Messages.Select(m => m.Id).ToList(),
                ChatUsers.Select(cu => cu.ChatId).ToList()
            );
        }
        
        public void MapEditUserDto(EditUserDto editUserDto)
        {
            Email = editUserDto.NewEmail;
            UserName = editUserDto.NewUserName;
            About = editUserDto.About;
        }
    }
}