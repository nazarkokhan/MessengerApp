using System.Threading.Tasks;

namespace MessengerApp.DAL.Repository.Abstraction
{
    public interface IUnitOfWork
    {
        public IUserRepository Users { get; }
        
        public IChatRepository Chats { get; }
        
        public IMessageRepository Message { get; }
        
        public IContactRepository Contact { get; }

        Task SaveAsync();
    }
}