using System.Threading.Tasks;

namespace MessengerApp.DAL.Repository.Abstraction
{
    public interface IUnitOfWork
    {
        IAuthorRepository Authors { get; }

        IBookRepository Books { get; }
        
        IUserRepository Users { get; }

        Task SaveAsync();
    }
}