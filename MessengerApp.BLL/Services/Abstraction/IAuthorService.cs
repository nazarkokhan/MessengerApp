using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Author;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;

namespace MessengerApp.BLL.Services.Abstraction
{
    public interface IAuthorService
    {
        Task<Result<Pager<AuthorDto>>> GetAuthorsAsync(int page, int itemsOnPage, string? search);

        Task<Result<AuthorDto>> GetAuthorAsync(int id);

        Task<Result<AuthorDto>> CreateAuthorAsync(CreateAuthorDto author);

        Task<Result<AuthorDto>> UpdateAuthorAsync(UpdateAuthorDto author);

        Task<Result> DeleteAuthorAsync(int id);
    }
}