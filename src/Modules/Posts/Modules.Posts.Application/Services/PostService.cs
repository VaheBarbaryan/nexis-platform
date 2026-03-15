using Modules.Posts.Application.Contracts;
using Modules.Posts.Domain;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Posts;
using Modules.Posts.Domain.Posts.Exceptions;
using Modules.Posts.Domain.Posts.Repositories;
using Modules.Posts.Domain.Posts.ValueObjects;

namespace Modules.Posts.Application.Services;

public sealed class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IPostUnitOfWork _unitOfWork;

    public PostService(
        IPostRepository postRepository,
        IPostUnitOfWork postUnitOfWork)
    {
        _postRepository = postRepository;
        _unitOfWork = postUnitOfWork;
    }

    public async Task<Post> CreateAsync(Guid authorId, string content, CancellationToken ct)
    {
        var post = Post.Create(new AuthorId(authorId), content);
        _postRepository.Add(post);

        await _unitOfWork.CommitAsync(ct);

        return post;
    }

    public async Task<Post> UpdateAsync(Guid authorId, Guid postId, string content, CancellationToken ct)
    {
        var postIdVo = new PostId(postId);
        var authorIdVo = new AuthorId(authorId);
        var post = await _postRepository.GetByIdAsync(postIdVo, ct);

        if (post is null)
        {
            throw new PostNotFoundException();
        }

        post.Update(authorIdVo, content);

        await _unitOfWork.CommitAsync(ct);

        return post;
    }
}
