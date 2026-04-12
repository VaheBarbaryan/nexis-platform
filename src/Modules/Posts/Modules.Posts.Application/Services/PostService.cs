using Modules.Posts.Application.Contracts;
using Modules.Posts.Domain;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Likes;
using Modules.Posts.Domain.Likes.Repositories;
using Modules.Posts.Domain.Posts;
using Modules.Posts.Domain.Posts.Exceptions;
using Modules.Posts.Domain.Posts.Repositories;
using Modules.Posts.Domain.Posts.ValueObjects;
using SharedKernel.Application.Pagination;

namespace Modules.Posts.Application.Services;

public sealed class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IPostLikeRepository _postLikeRepository;
    private readonly IPostUnitOfWork _unitOfWork;

    public PostService(
        IPostRepository postRepository,
        IPostLikeRepository postLikeRepository,
        IPostUnitOfWork postUnitOfWork)
    {
        _postRepository = postRepository;
        _postLikeRepository = postLikeRepository;
        _unitOfWork = postUnitOfWork;
    }

    public async Task<CursorResponse<PostSummary>> GetPostsAsync(string? cursor, int limit = 10,
        CancellationToken ct = default)
    {
        var posts = await _postRepository.GetPostsAsync(cursor, limit, ct);
        var counts = await _postLikeRepository.GetCountsAsync(posts.Select(p => p.Id), ct);

        return CursorResponse.From(
            posts,
            limit,
            p => new PostSummary(p.Id.Value, p.AuthorId.Value, p.Content, p.CreatedAt, counts.GetValueOrDefault(p.Id.Value)),
            p => new Cursor(p.CreatedAt, p.Id.Value).Encode());
    }

    public async Task<PostDetail> GetByIdAsync(Guid postId, CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(new PostId(postId), ct)
            ?? throw new PostNotFoundException();

        var likesCount = await _postLikeRepository.GetCountAsync(post.Id, ct);

        return new PostDetail(post.Id.Value, post.AuthorId.Value, post.Content, post.CreatedAt, post.UpdatedAt, likesCount);
    }

    public async Task<Post> CreateAsync(Guid authorId, string content, CancellationToken ct = default)
    {
        var post = Post.Create(new AuthorId(authorId), content);
        _postRepository.Add(post);

        await _unitOfWork.CommitAsync(ct);

        return post;
    }

    public async Task<Post> UpdateAsync(Guid authorId, Guid postId, string content, CancellationToken ct = default)
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

    public async Task DeleteAsync(Guid authorId, Guid postId, CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(new PostId(postId), ct);

        if (post is null)
            throw new PostNotFoundException();

        post.Delete(new AuthorId(authorId));

        await _unitOfWork.CommitAsync(ct);
    }

    public async Task LikeAsync(Guid authorId, Guid postId, CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(new PostId(postId), ct)
                   ?? throw new PostNotFoundException();

        var existing = await _postLikeRepository.GetAsync(post.Id, new AuthorId(authorId), ct);
        if (existing is not null) return;

        var like = PostLike.Create(post.Id, new AuthorId(authorId));
        _postLikeRepository.Add(like);

        await _unitOfWork.CommitAsync(ct);
        await _postLikeRepository.RefreshCountsAsync(ct);
    }

    public async Task UnlikeAsync(Guid authorId, Guid postId, CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(new PostId(postId), ct)
                   ?? throw new PostNotFoundException();

        var existing = await _postLikeRepository.GetAsync(post.Id, new AuthorId(authorId), ct);
        if (existing is null) return;

        _postLikeRepository.Remove(existing);

        await _unitOfWork.CommitAsync(ct);
        await _postLikeRepository.RefreshCountsAsync(ct);
    }
}
