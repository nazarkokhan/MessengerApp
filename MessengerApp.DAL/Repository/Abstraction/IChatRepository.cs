using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Entities;

namespace MessengerApp.DAL.Repository.Abstraction
{
    public interface IChatRepository
    {
        Task<Result<Pager<Chat>>> GetСhatsPerUserPageAsync(
            int userId, string? search, int page, int items);
    }
}