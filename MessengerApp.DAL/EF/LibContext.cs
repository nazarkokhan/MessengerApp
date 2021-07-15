using MessengerApp.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MessengerApp.DAL.EF
{
    public class LibContext : IdentityDbContext<User, Role, int>
    {
        public LibContext(DbContextOptions<LibContext> options) : base(options)
        {
        }

        public override DbSet<User> Users { get; set; }
    }
}