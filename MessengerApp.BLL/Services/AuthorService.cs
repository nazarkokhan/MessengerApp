using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Author;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Repository.Abstraction;
using MessengerApp.BLL.Services.Abstraction;

namespace MessengerApp.BLL.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Result<Pager<AuthorDto>>> GetAuthorsAsync(int page, int itemsOnPage, string? search) 
            => _unitOfWork.Authors.GetAuthorsAsync(page, itemsOnPage, search);

        public Task<Result<AuthorDto>> GetAuthorAsync(int id) 
            => _unitOfWork.Authors.GetAuthorAsync(id);

        public Task<Result<AuthorDto>> CreateAuthorAsync(CreateAuthorDto author) 
            => _unitOfWork.Authors.CreateAuthorAsync(author);

        public Task<Result<AuthorDto>> UpdateAuthorAsync(UpdateAuthorDto author) 
            => _unitOfWork.Authors.UpdateAuthorAsync(author);

        public Task<Result> DeleteAuthorAsync(int id) 
            => _unitOfWork.Authors.DeleteAuthorAsync(id);
    }
}