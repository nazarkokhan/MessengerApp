using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MessengerApp.Core.DTO.User;
using MessengerApp.DAL.Entities.Abstract;
using Microsoft.AspNetCore.Identity;

// ReSharper disable All
#pragma warning disable 8618

namespace MessengerApp.DAL.Entities.Authorization
{
    public class User : IdentityUser<int>, IEntity<int>
    {
        /// <summary>Is unique.</summary>
        [Required]
        [MaxLength(10)]
        public override string UserName { get; set; }

        public int Age { get; set; }
        
        [MaxLength(512)]
        public string About { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        public override string Email { get; set; }
        
        public List<Contact> Contacts { get; set; }

        public ICollection<Message> Messages { get; set; }

        [DataMember]
        public ICollection<ChatUser> ChatUsers { get; set; }

        public UserContactDto MapUserContactDto()
        {
            return new UserContactDto(UserName, About, Email);
        }
    }
}