using System.Threading.Tasks;
using MessengerApp.DAL.EF;
using MessengerApp.DAL.Repository.Abstraction;
// ReSharper disable All
#pragma warning disable 8618

namespace MessengerApp.DAL.Repository
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly MsgContext _db;

        public EfUnitOfWork(MsgContext context, IUserRepository users, 
            IChatRepository chats, IMessageRepository message, IContactRepository contact)
        {
            _db = context;
            Users = users;
            Chats = chats;
            Message = message;
            Contact = contact;
        }
        
        public IUserRepository Users { get; }
        
        public IChatRepository Chats { get; }
        
        public IMessageRepository Message { get; }
        
        public IContactRepository Contact { get; }

        public Task SaveAsync() 
            => _db.SaveChangesAsync();
    }
}