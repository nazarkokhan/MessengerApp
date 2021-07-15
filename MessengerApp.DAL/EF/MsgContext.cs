using MessengerApp.DAL.Entities;
using MessengerApp.DAL.Entities.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MessengerApp.DAL.EF
{
    public class MsgContext : IdentityDbContext<User, Role, int>
    {
        public MsgContext(DbContextOptions<MsgContext> options) : base(options)
        {
        }

        public DbSet<Chat> Chats { get; set; }
        
        public override DbSet<User> Users { get; set; }
        
        public DbSet<ChatUser> ChatUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(u => u.NickName)
                .IsUnique();
        }
    }
}