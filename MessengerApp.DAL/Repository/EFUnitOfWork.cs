using System.Threading.Tasks;
using MessengerApp.DAL.EF;
using MessengerApp.DAL.Repository.Abstraction;

namespace MessengerApp.DAL.Repository
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly LibContext _db;

        public EfUnitOfWork(LibContext context, IUserRepository users)
        {
            _db = context;
            Users = users;
        }

        public IUserRepository Users { get; }

        public Task SaveAsync() 
            => _db.SaveChangesAsync();
    }
}