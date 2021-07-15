using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Book;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;

namespace MessengerApp.DAL.Repository.Abstraction
{
    public interface IBookRepository
    {
        Task<Result<Pager<BookDto>>> GetBooksAsync(int page, int itemsOnPage, string? search);

        Task<Result<BookDto>> GetBookAsync(int id);

        Task<Result<BookDto>> CreateBookAsync(CreateBookDto book);

        Task<Result<BookDto>> UpdateBookAsync(UpdateBookDto book);

        Task<Result> DeleteBookAsync(int id);
    }
}