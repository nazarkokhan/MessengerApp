using System.Threading.Tasks;

namespace MessengerApp.DAL.Repository.Abstraction
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }

        Task SaveAsync();
    }
}