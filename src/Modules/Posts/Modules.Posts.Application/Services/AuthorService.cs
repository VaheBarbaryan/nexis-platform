using Modules.Posts.Application.Contracts;
using Modules.Posts.Domain;
using Modules.Posts.Domain.Authors;
using Modules.Posts.Domain.Authors.Repositories;

namespace Modules.Posts.Application.Services;

public sealed class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IPostUnitOfWork _unitOfWork;

    public AuthorService(
        IAuthorRepository authorRepository,
        IPostUnitOfWork unitOfWork
    )
    {
        _authorRepository = authorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Author> CreateAsync(Guid userId, string username, CancellationToken ct)
    {
        var author = Author.Create(userId, username);

        _authorRepository.Add(author);

        await _unitOfWork.CommitAsync(ct);

        return author;
    }
}
