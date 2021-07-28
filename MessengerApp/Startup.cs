using MessengerApp.BLL.Services;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.DAL.EF;
using MessengerApp.DAL.Entities.Authorization;
using MessengerApp.DAL.Repository;
using MessengerApp.DAL.Repository.Abstraction;
using MessengerApp.Extensions;
using MessengerApp.Filters;
using MessengerApp.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessengerApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication()
                .AddJwtBearer(options => options.JwtBearerOptions());

            services
                .AddIdentity<User, Role>(options => options.ConfigurePassword())
                .AddUserManager<UserManager<User>>()
                .AddEntityFrameworkStores<MsgContext>()
                .AddDefaultTokenProviders();

            services
                .AddDbContext<MsgContext>(options => options
                    .UseSqlServer(Configuration.GetConnectionString(nameof(MsgContext)))
                    .UseLoggerFactory(LoggerFactory.Create(lb => lb.AddConsole()))
                );

            services
                .AddHttpContextAccessor()
                .AddScoped<DataBaseInitializer>()
                .AddScoped<IUnitOfWork, EfUnitOfWork>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IContactRepository, ContactRepository>()
                .AddScoped<IChatRepository, ChatRepository>()
                .AddScoped<IMessageRepository, MessageRepository>()
                .AddScoped<IAccountService, AccountService>()
                .AddScoped<IAdminService, AdminService>()
                .AddScoped<ITokenService, TokenService>()
                .AddScoped<IContactService, ContactService>()
                .AddScoped<IChatService, ChatService>()
                .AddScoped<IMessageService, MessageService>()
                .AddSingleton<IEmailService, EmailService>()
                .AddLogging(builder => builder.AddFile(Configuration.GetLogFileName(), fileSizeLimitBytes: 100_000))
                .AddControllers(options => options.Filters.Add<ErrorableResultFilterAttribute>());

            services
                .AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) 
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                
                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }
}