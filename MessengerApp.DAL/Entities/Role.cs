using MessengerApp.DAL.Entities.Abstract;
using Microsoft.AspNetCore.Identity;

namespace MessengerApp.DAL.Entities
{
    public class Role : IdentityRole<int>, IEntity<int>
    {
        public string? RoleDescription { get; set; }
    }
}