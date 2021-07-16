using MessengerApp.DAL.Entities.Abstract;
using Microsoft.AspNetCore.Identity;

// ReSharper disable All
#pragma warning disable 8618

namespace MessengerApp.DAL.Entities.Authorization
{
    public class Role : IdentityRole<int>, IEntity<int>
    {
        public string RoleDescription { get; set; }
    }
}