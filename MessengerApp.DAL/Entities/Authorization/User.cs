using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MessengerApp.DAL.Entities.Abstract;
using Microsoft.AspNetCore.Identity;

namespace MessengerApp.DAL.Entities.Authorization
{
    public class User : IdentityUser<int>, IEntity<int>
    {
        /// <summary>
        /// Is unique.
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string NickName { get; set; }

        public int Age { get; set; }
        
        [MaxLength(512)]
        public string About { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        public override string Email { get; set; }
        
        // public List<int> 
        
        [DataMember]
        public ICollection<ChatUser> ChatUsers { get; set; }
    }
}