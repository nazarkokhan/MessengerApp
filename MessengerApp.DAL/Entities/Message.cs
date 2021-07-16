using System;
using System.ComponentModel.DataAnnotations;
using MessengerApp.DAL.Entities.Abstract;
using MessengerApp.DAL.Entities.Authorization;

#pragma warning disable 8618

namespace MessengerApp.DAL.Entities
{
    public class Message : IEntity<long>
    {
        private DateTime _dateTime;
        
        public long Id { get; set; }
        
        [Required]
        public string Body { get; set; }

        public DateTime DateTime
        {
            get => _dateTime;
            set => _dateTime = DateTime.UtcNow;
        }
        
        [Required]
        public int UserId { get; set; }

        [Required]
        public int ChatId { get; set; }
    }
}