using System.Collections.Generic;
using System.Threading.Tasks;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.Extensions;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.DAL.EF;
using MessengerApp.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace MessengerApp.DAL.Repository
{
    public class DataBaseInitializer
    {
        private readonly LibContext _db;

        private readonly RoleManager<Role> _roleManager;

        private readonly UserManager<User> _userManager;
        
        private readonly List<Role> _roles;

        private readonly List<RegisterDto> _users;
        
        private readonly IHostEnvironment _hostEnvironment;

        public DataBaseInitializer(LibContext db, RoleManager<Role> roleManager, 
            UserManager<User> userManager, IHostEnvironment hostEnvironment)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;

            _roles ??= new List<Role>
            {
                new() {Name = Roles.Admin.ToString(), RoleDescription = "Has a admin access"},
                new() {Name = Roles.User.ToString(), RoleDescription = "Roles for all registered users"}
            };

            _users ??= new List<RegisterDto>
            {
                new("admin@gmail.com", "adminAccess", 99),
                new("guest@gmail.com", "guestAccess", 99),
                new("user@gmail.com", "userAccess", 99)
            };
        }

        public async Task InitializeDbAsync()
        {
            if (_hostEnvironment.IsDevelopment())
            {
                await _db.Database.EnsureCreatedAsync();

                await InitializeRolesAsync();

                await InitializeUsersAsync();
            }
        }

        private async Task InitializeRolesAsync()
        {
            if (!await _roleManager.Roles.AnyAsync())
                await _roles.ForEachAsync(async r => await _roleManager.CreateAsync(r));
        }

        private async Task InitializeUsersAsync()
        {
            if (!await _userManager.Users.AnyAsync())
                await _users.ForEachAsync(async u =>
                {
                    var user = new User
                    {
                        Email = u.Email,
                        UserName = u.Email,
                        Age = u.Age
                    };

                    await _userManager.CreateAsync(user, u.Password);

                    user.EmailConfirmed = true;
                    
                    if (user.Id != 1)
                        await _userManager.AddToRoleAsync(user, Roles.User.ToString());

                    else
                        await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
                });
        }
    }
}