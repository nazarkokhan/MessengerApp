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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Contact>()
                .HasKey(c => new {c.UserId, c.UserContactId});

            builder.Entity<Contact>()
                .HasOne<User>(c => c.User)
                .WithMany(u => u.UserContacts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.Entity<Contact>()
                .HasOne<User>(c => c.UserContact)
                .WithMany(u => u.ContactUsers)
                .HasForeignKey(c => c.UserContactId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}