using Modules.Posts.Application.Contracts;
using Modules.Posts.Domain;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Posts;
using Modules.Posts.Domain.Posts.Repositories;

namespace Modules.Posts.Application.Services;

public sealed class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IPostUnitOfWork _postUnitOfWork;

    public PostService(
        IPostRepository postRepository,
        IPostUnitOfWork postUnitOfWork)
    {
        _postRepository = postRepository;
        _postUnitOfWork = postUnitOfWork;
    }

    public async Task<Post> CreateAsync(Guid authorId, string content, CancellationToken ct)
    {
        var post = Post.Create(new AuthorId(authorId), content);
        _postRepository.Add(post);

        await _postUnitOfWork.CommitAsync(ct);

        return post;
    }
}
