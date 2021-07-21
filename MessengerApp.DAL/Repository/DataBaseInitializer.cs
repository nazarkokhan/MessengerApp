using System.Collections.Generic;
using System.Threading.Tasks;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.Extensions;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using MessengerApp.DAL.EF;
using MessengerApp.DAL.Entities;
using MessengerApp.DAL.Entities.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace MessengerApp.DAL.Repository
{
    public class DataBaseInitializer
    {
        private readonly MsgContext _db;

        private readonly RoleManager<Role> _roleManager;

        private readonly UserManager<User> _userManager;

        private readonly List<Chat> _chats;

        private readonly List<Message> _messages;

        private readonly List<Role> _roles;

        private readonly List<RegisterDto> _register;

        private readonly IHostEnvironment _hostEnvironment;

        public DataBaseInitializer(MsgContext db, RoleManager<Role> roleManager,
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

            _register ??= new List<RegisterDto>
            {
                new("admin", "admin@gmail.com", "adminAccess"),
                new("guest", "guest@gmail.com", "guestAccess"),
                new("user", "user@gmail.com", "userAccess")
            };

            _chats ??= new List<Chat>
            {
                new()
                {
                    Name = "TomChat",
                    AdminId = 1,
                    ChatUsers = new List<ChatUser>()
                    {
                        new() {UserId = 1}
                    }
                },
                new()
                {
                    Name = "AliceChat",
                    AdminId = 2,
                    ChatUsers = new List<ChatUser>()
                    {
                        new() {UserId = 2}
                    }
                },
                new()
                {
                    Name = "JohnChat",
                    AdminId = 3,
                    ChatUsers = new List<ChatUser>()
                    {
                        new() {UserId = 3}
                    }
                }
            };

            _messages ??= new List<Message>
            {
                new() {Body = "Hi", UserId = 1, ChatId = 1},
                new() {Body = "Alo", UserId = 2, ChatId = 2},
                new() {Body = "Im here", UserId = 3, ChatId = 3}
            };
        }

        public async Task InitializeDbAsync()
        {
            if (_hostEnvironment.IsDevelopment())
            {
                await _db.Database.EnsureCreatedAsync();

                await InitializeRolesAsync();

                await InitializeUsersAsync();

                await InitializeContactsAsync();

                await InitializeChatsAsync();

                await InitializeMessagesAsync();
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
                await _register.ForEachAsync(async u =>
                {
                    var user = new User
                    {
                        Email = u.Email,
                        UserName = u.UserName,
                        About = "no data"
                    };

                    await _userManager.CreateAsync(user, u.Password);

                    user.EmailConfirmed = true;

                    if (user.Id != 1)
                        await _userManager.AddToRoleAsync(user, Roles.User.ToString());

                    else
                        await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());

                    await _db.SaveChangesAsync();
                });
        }

        private async Task InitializeContactsAsync()
        {
            if (!await _db.Contacts.AnyAsync())
            {
                var contacts = new List<Contact>();

                var count = await _db.Users.CountAsync();

                var users = await _db.Users
                    .TakePage(1, 10)
                    .ToListAsync();

                for (var i = 0; i < count; i++)
                {
                    for (var j = 0; j < count; j++)
                    {
                        contacts.Add(new Contact
                        {
                            UserId = users[i].Id,
                            UserContactId = users[j].Id
                        });
                    }
                }

                await _db.Contacts.AddRangeAsync(contacts);

                await _db.SaveChangesAsync();
            }
        }

        private async Task InitializeChatsAsync()
        {
            if (!await _db.Chats.AnyAsync())
            {
                await _db.Chats.AddRangeAsync(_chats);

                await _db.SaveChangesAsync();
            }
        }

        private async Task InitializeMessagesAsync()
        {
            if (!await _db.Messages.AnyAsync())
            {
                await _db.Messages.AddRangeAsync(_messages);

                await _db.SaveChangesAsync();
            }
        }
    }
}