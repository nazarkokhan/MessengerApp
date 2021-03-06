using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MessengerApp.Core.DTO.Authorization;
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
        [Required] [MaxLength(10)] public override string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public override string Email { get; set; }

        [MaxLength(512)] 
        public string? About { get; set; }

        public ICollection<Contact> UserContacts { get; set; }

        public ICollection<Contact> ContactUsers { get; set; }

        public ICollection<Message> Messages { get; set; }

        [DataMember] public ICollection<ChatUser> ChatUsers { get; set; }

        public ContactDto MapUserContactDto()
        {
            return new ContactDto(Id, UserName, About, Email);
        }

        public UserDto MapUserDto()
        {
            return new UserDto(Id, UserName, About, Email);
        }

        public void MapEditUserDto(EditUserDto editUserDto)
        {
            UserName = editUserDto.UserName;
            About = editUserDto.About;
        }
        
        public void MapEditUserByAdminDto(EditUserByAdminDto editUserByAdminDto)
        {
            Email = editUserByAdminDto.NewEmail;
            UserName = editUserByAdminDto.NewUserName;
            About = editUserByAdminDto.About;
        }
    }
}