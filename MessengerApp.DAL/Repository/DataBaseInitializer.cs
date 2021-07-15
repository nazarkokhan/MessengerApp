using System.Collections.Generic;
using System.Threading.Tasks;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.Extensions;
using MessengerApp.DAL.EF;
using MessengerApp.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Role = MessengerApp.Core.ResultConstants.AuthorizationConstants.Role;

namespace MessengerApp.DAL.Repository
{
    public class DataBaseInitializer
    {
        private readonly LibContext _db;

        private readonly RoleManager<Entities.Role> _roleManager;

        private readonly UserManager<User> _userManager;
        
        private readonly List<Author> _authors;

        private readonly List<Book> _books;

        private readonly List<Entities.Role> _roles;

        private readonly List<RegisterDto> _users;
        
        private readonly IHostEnvironment _hostEnvironment;

        public DataBaseInitializer(LibContext db, RoleManager<Entities.Role> roleManager, 
            UserManager<User> userManager, IHostEnvironment hostEnvironment)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;

            _authors ??= new List<Author>
            {
                new() {Name = "Peter"},
                new() {Name = "Alice"},
                new() {Name = "John"}
            };

            _books ??= new List<Book>
            {
                new() {Name = "Kolobok"},
                new() {Name = "Voina I Mir"},
                new() {Name = "Tri Porosenka"}
            };

            _roles ??= new List<Entities.Role>
            {
                new() {Name = Role.Admin.ToString(), RoleDescription = "Has a admin access"},
                new() {Name = Role.User.ToString(), RoleDescription = "Role for all registered users"}
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

                await InitializeAuthorsAsync();

                await InitializeBooksAsync();

                await InitializeAuthorBooksAsync();

                await InitializeRolesAsync();

                await InitializeUsersAsync();
            }
        }

        private async Task InitializeBooksAsync()
        {
            if (!await _db.Books.AnyAsync())
            {
                await _db.Books.AddRangeAsync(_books);

                await _db.SaveChangesAsync();
            }
        }

        private async Task InitializeAuthorsAsync()
        {
            if (!await _db.Authors.AnyAsync())
            {
                await _db.Authors.AddRangeAsync(_authors);

                await _db.SaveChangesAsync();
            }
        }

        private async Task InitializeAuthorBooksAsync()
        {
            if (!await _db.AuthorBooks.AnyAsync())
            {
                var authorBooks = new List<AuthorBook>();

                var aCount = await _db.Authors.CountAsync();

                var bCount = await _db.Books.CountAsync();

                var count = aCount >= bCount ? aCount : bCount;

                for (var i = 0; i < count; i++)
                    authorBooks.Add(new AuthorBook {BookId = _books[i].Id, AuthorId = _authors[i].Id});

                await _db.AuthorBooks.AddRangeAsync(authorBooks);

                await _db.SaveChangesAsync();
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
                        await _userManager.AddToRoleAsync(user, Role.User.ToString());

                    else
                        await _userManager.AddToRoleAsync(user, Role.Admin.ToString());
                });
        }
    }
}