using System.Threading.Tasks;
using MessengerApp.DAL.EF;
using MessengerApp.DAL.Repository.Abstraction;

namespace MessengerApp.DAL.Repository
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly MsgContext _db;

        public EfUnitOfWork(MsgContext context, IChatRepository chats, IUserRepository users)
        {
            _db = context;
            Chats = chats;
            Users = users;
        }

        public IChatRepository Chats { get; }
        
        public IUserRepository Users { get; }

        public Task SaveAsync() 
            => _db.SaveChangesAsync();
    }
}