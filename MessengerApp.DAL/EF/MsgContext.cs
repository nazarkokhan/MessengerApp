using MessengerApp.DAL.Entities;
using MessengerApp.DAL.Entities.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// ReSharper disable All
#pragma warning disable 8618

namespace MessengerApp.DAL.EF
{
    public class MsgContext : IdentityDbContext<User, Role, int>
    {
        public MsgContext(DbContextOptions<MsgContext> options) : base(options)
        {
        }

        public override DbSet<User> Users { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<ChatUser> ChatUsers { get; set; }
        
        public DbSet<Message> Messages { get; set; }
        
        public DbSet<Contact> Contacts { get; set; }
    }
}