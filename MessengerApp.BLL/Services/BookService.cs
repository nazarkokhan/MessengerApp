using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Book;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Repository.Abstraction;
using MessengerApp.BLL.Services.Abstraction;

namespace MessengerApp.BLL.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Result<Pager<BookDto>>> GetBooksAsync(int page, int itemsOnPage, string? search) 
            => _unitOfWork.Books.GetBooksAsync(page, itemsOnPage, search);

        public Task<Result<BookDto>> GetBookAsync(int id) 
            => _unitOfWork.Books.GetBookAsync(id);

        public Task<Result<BookDto>> CreateBookAsync(CreateBookDto book) 
            => _unitOfWork.Books.CreateBookAsync(book);

        public Task<Result<BookDto>> UpdateBookAsync(UpdateBookDto book) 
            => _unitOfWork.Books.UpdateBookAsync(book);

        public Task<Result> DeleteBookAsync(int id) 
            => _unitOfWork.Books.DeleteBookAsync(id);
    }
}