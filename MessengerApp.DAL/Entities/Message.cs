using System;
using System.ComponentModel.DataAnnotations;
using MessengerApp.Core.DTO.Message;
using MessengerApp.DAL.Entities.Abstract;
using MessengerApp.DAL.Entities.Authorization;

// ReSharper disable All

#pragma warning disable 8618

namespace MessengerApp.DAL.Entities
{
    public class Message : IEntity<long>
    {
        public Message()
        {
            DateTime = DateTime.Now;
        }
        
        public long Id { get; set; }
        
        [Required]
        public string Body { get; set; }

        public DateTime DateTime { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        public int ChatId { get; set; }

        public Chat Chat { get; set; }

        public MessageDto MapMessageDto()
        {
            return new MessageDto(Id, Body, DateTime, UserId, ChatId);
        }
        public void MapEditMessageDto(EditMessageDto editMessageDto)
        {
            Body = editMessageDto.Body;
        }
    }
}