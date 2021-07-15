using System;
using System.Linq;
using System.Threading.Tasks;
using MessengerApp.Core.DTO;
using MessengerApp.Core.DTO.Author;
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
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibContext _db;

        public AuthorRepository(LibContext context)
        {
            _db = context;
        }

        public async Task<Result<Pager<AuthorDto>>> GetAuthorsAsync(int page, int items, string? search)
        {
            try
            {
                var totalCount = await _db.Authors.CountAsync();

                var authors = _db.Authors
                    .OrderBy(a => a.Id)
                    .TakePage(page, items);

                if (!string.IsNullOrWhiteSpace(search))
                    authors = authors
                        .Where(a => a.Name.Contains(search));

                return Result<Pager<AuthorDto>>.CreateSuccess(
                    new Pager<AuthorDto>(
                        await authors
                            .Select(a => new AuthorDto(a.Id, a.Name))
                            .ToListAsync(), totalCount
                    )
                );
            }
            catch (Exception e)
            {
                return Result<Pager<AuthorDto>>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<AuthorDto>> GetAuthorAsync(int id)
        {
            try
            {
                var author = await _db.Authors
                    .Where(a => a.Id == id)
                    .Select(a => new AuthorDto(a.Id, a.Name))
                    .FirstOrDefaultAsync();

                return author is null
                    ? Result<AuthorDto>.CreateFailed(
                        AuthorRepositoryResultConstants.AuthorNotFound,
                        new NullReferenceException()
                    )
                    : Result<AuthorDto>.CreateSuccess(author);
            }
            catch (Exception e)
            {
                return Result<AuthorDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<AuthorDto>> CreateAuthorAsync(CreateAuthorDto author)
        {
            try
            {
                var authorEntity = new Author
                {
                    Name = author.Name
                };

                await _db.Authors.AddAsync(authorEntity);

                await _db.SaveChangesAsync();

                return Result<AuthorDto>.CreateSuccess(
                    new AuthorDto(
                        authorEntity.Id,
                        authorEntity.Name
                    )
                );
            }
            catch (Exception e)
            {
                return Result<AuthorDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result<AuthorDto>> UpdateAuthorAsync(UpdateAuthorDto author)
        {
            try
            {
                var authorEntity = await _db.Authors
                    .FirstOrDefaultAsync(a => a.Id == author.Id);

                if (authorEntity is null)
                {
                    return Result<AuthorDto>.CreateFailed(
                        AuthorRepositoryResultConstants.AuthorNotFound,
                        new NullReferenceException()
                    );
                }

                authorEntity.Name = author.Name;

                await _db.SaveChangesAsync();

                return Result<AuthorDto>.CreateSuccess(
                    new AuthorDto(
                        authorEntity.Id,
                        authorEntity.Name
                    )
                );
            }
            catch (Exception e)
            {
                return Result<AuthorDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<Result> DeleteAuthorAsync(int id)
        {
            try
            {
                var authorEntity = await _db.Authors
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (authorEntity is null)
                    return Result.CreateFailed(
                        AuthorRepositoryResultConstants.AuthorNotFound,
                        new NullReferenceException()
                    );

                _db.Authors.Remove(authorEntity);

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