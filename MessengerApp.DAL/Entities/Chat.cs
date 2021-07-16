using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MessengerApp.Core.DTO.Chat;
using MessengerApp.DAL.Entities.Abstract;
// ReSharper disable All
#pragma warning disable 8618

namespace MessengerApp.DAL.Entities
{
    public class Chat : EntityBase
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int AdminId { get; set; }

        public ICollection<Message> Messages { get; set; }
        
        [DataMember] 
        public ICollection<ChatUser> ChatUsers { get; set; }

        public ChatDto MapChatDto()
        {
            return new ChatDto(Name);
        }
        
        public void MapEditChatDto(EditChatDto editChatDto)
        {
            Name = editChatDto.NewName;
            AdminId = editChatDto.AdminId;
        }
    }
}