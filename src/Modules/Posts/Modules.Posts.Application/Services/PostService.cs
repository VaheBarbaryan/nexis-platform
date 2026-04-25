using Modules.Posts.Application.Contracts;
using Modules.Posts.Domain;
using Modules.Posts.Domain.Authors;
using Modules.Posts.Domain.Authors.Repositories;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Comments.Repositories;
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
    private readonly IAuthorRepository _authorRepository;
    private readonly IPostLikeRepository _postLikeRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IPostUnitOfWork _unitOfWork;

    public PostService(
        IPostRepository postRepository,
        IAuthorRepository authorRepository,
        IPostLikeRepository postLikeRepository,
        ICommentRepository commentRepository,
        IPostUnitOfWork postUnitOfWork)
    {
        _postRepository = postRepository;
        _authorRepository = authorRepository;
        _postLikeRepository = postLikeRepository;
        _commentRepository = commentRepository;
        _unitOfWork = postUnitOfWork;
    }

    public async Task<CursorResponse<PostSummary>> GetPostsAsync(string? cursor, int limit = 10,
        CancellationToken ct = default)
    {
        var posts = await _postRepository.GetPostsAsync(cursor, limit, ct);
        var postIds = posts.Select(p => p.Id).ToList();

        var likeCounts = await _postLikeRepository.GetCountsAsync(postIds, ct);
        var commentCounts = await _commentRepository.GetCountsAsync(postIds, ct);

        var authors = await _authorRepository.GetByIdsAsync(posts.Select(c => c.AuthorId), ct);

        return CursorResponse.From(
            posts,
            limit,
            p =>
            {
                authors.TryGetValue(p.AuthorId, out var author);
                return new PostSummary(
                    p.Id.Value,
                    new PostAuthor(p.AuthorId.Value, author?.Username ?? string.Empty),
                    p.Content,
                    p.CreatedAt,
                    likeCounts.GetValueOrDefault(p.Id.Value),
                    commentCounts.GetValueOrDefault(p.Id.Value));
            },
            p => new Cursor(p.CreatedAt, p.Id.Value).Encode());
    }

    public async Task<PostDetail> GetByIdAsync(Guid postId, CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(new PostId(postId), ct)
                   ?? throw new PostNotFoundException();

        var likesCount = await _postLikeRepository.GetCountAsync(post.Id, ct);
        var commentsCount = await _commentRepository.GetCountAsync(post.Id, ct);

        var author = await _authorRepository.GetByIdAsync(post.AuthorId, ct);

        return new PostDetail(post.Id.Value, new PostAuthor(post.AuthorId.Value, author?.Username ?? string.Empty),
            post.Content, post.CreatedAt, post.UpdatedAt,
            likesCount, commentsCount);
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
