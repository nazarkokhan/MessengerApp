using System;
using System.Linq;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Author;
using MessengerApp.Core.DTO.Book;
using MessengerApp.Core.Extensions;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultModel;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.EF;
using MessengerApp.DAL.Entities;
using MessengerApp.DAL.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace MessengerApp.DAL.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly LibContext _db;

        public BookRepository(LibContext context)
        {
            _db = context;
        }

        public async Task<Result<Pager<BookDto>>> GetBooksAsync(int page, int items, string? search)
        {
            try
            {
                var totalCount = await _db.Books.CountAsync();

                var books = _db.Books
                    .OrderBy(a => a.Id)
                    .TakePage(page, items);

                if (!string.IsNullOrWhiteSpace(search))
                    books = books
                        .Where(b => b.Name.Contains(search));

                return Result<Pager<BookDto>>.CreateSuccess(
                    new Pager<BookDto>(await books.Select(b => new BookDto(
                        b.Id,
                        b.Name,
                        b.AuthorBooks.Select(ab => new AuthorDto(
                            ab.AuthorId,
                            ab.Author.Name)
                        ))
                    ).ToListAsync(), totalCount)
                );
            }
            catch (Exception e)
            {
                return Result<Pager<BookDto>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<BookDto>> GetBookAsync(int id)
        {
            try
            {
                var book = await _db.Books
                    .Where(b => b.Id == id)
                    .Select(b => new BookDto(
                        b.Id,
                        b.Name,
                        b.AuthorBooks.Select(ab => new AuthorDto(
                            ab.AuthorId,
                            ab.Author.Name)
                        ).ToList())
                    ).FirstOrDefaultAsync();

                return book is null
                    ? Result<BookDto>.CreateFailed(
                        BookRepositoryResultConstants.BookNotFound,
                        new NullReferenceException()
                    )
                    : Result<BookDto>.CreateSuccess(book);
            }
            catch (Exception e)
            {
                return Result<BookDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<BookDto>> CreateBookAsync(CreateBookDto book)
        {
            try
            {
                var bookEntity = new Book
                {
                    Name = book.Name,
                    AuthorBooks = book.AuthorIds.Select(aId => new AuthorBook
                    {
                        AuthorId = aId
                    }).ToList()
                };

                await _db.Books.AddAsync(bookEntity);

                await _db.SaveChangesAsync();

                await _db.Entry(bookEntity)
                    .Collection(b => b.AuthorBooks)
                    .Query()
                    .Include(ab => ab.Author)
                    .LoadAsync();

                return Result<BookDto>.CreateSuccess(
                    new BookDto(
                        bookEntity.Id,
                        bookEntity.Name,
                        bookEntity.AuthorBooks.Select(ab => new AuthorDto(
                            ab.AuthorId,
                            ab.Author.Name)
                        ).ToList()
                    ));
            }
            catch (Exception e)
            {
                return Result<BookDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<BookDto>> UpdateBookAsync(UpdateBookDto book)
        {
            try
            {
                var bookEntity = await _db.Books
                    .Include(b => b.AuthorBooks)
                    .FirstOrDefaultAsync(b => b.Id == book.Id);

                if (bookEntity is null)
                {
                    return Result<BookDto>.CreateFailed(
                        BookRepositoryResultConstants.BookNotFound,
                        new NullReferenceException()
                    );
                }

                bookEntity.Name = book.Name;

                bookEntity.AuthorBooks = book.AuthorIds.Select(aId => new AuthorBook
                {
                    AuthorId = aId
                }).ToList();

                await _db.SaveChangesAsync();

                await _db.Entry(bookEntity)
                    .Collection(b => b.AuthorBooks)
                    .Query()
                    .Include(ab => ab.Author)
                    .LoadAsync();

                return Result<BookDto>.CreateSuccess(
                    new BookDto(
                        bookEntity.Id,
                        bookEntity.Name,
                        bookEntity.AuthorBooks.Select(ab => new AuthorDto(
                            ab.AuthorId,
                            ab.Author.Name)
                        )
                    )
                );
            }
            catch (Exception e)
            {
                return Result<BookDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result> DeleteBookAsync(int id)
        {
            try
            {
                var bookEntity = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);

                if (bookEntity is null)
                {
                    return Result.CreateFailed(
                        BookRepositoryResultConstants.BookNotFound,
                        new NullReferenceException()
                    );
                }

                _db.Books.Remove(bookEntity);

                await _db.SaveChangesAsync();

                return Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
    }
}