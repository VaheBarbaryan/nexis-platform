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
                    new PostAuthor(p.AuthorId.Value, author?.Username.Value ?? string.Empty),
                    p.Content.Value,
                    p.CreatedAt,
                    likeCounts.GetValueOrDefault(p.Id.Value),
                    commentCounts.GetValueOrDefault(p.Id.Value),
                    IsLiked: false);
            },
            p => new Cursor(p.CreatedAt, p.Id.Value).Encode());
    }

    public async Task<CursorResponse<PostSummary>> GetFeedAsync(Guid userId, string? cursor, int limit = 10,
        CancellationToken ct = default)
    {
        var authorId = AuthorId.From(userId);
        var posts = await _postRepository.GetFeedAsync(authorId, cursor, limit, ct);
        var postIds = posts.Select(p => p.Id).ToList();

        var likeCounts = await _postLikeRepository.GetCountsAsync(postIds, ct);
        var commentCounts = await _commentRepository.GetCountsAsync(postIds, ct);
        var likedSet = await _postLikeRepository.GetLikedByUserAsync(postIds, authorId, ct);

        var authors = await _authorRepository.GetByIdsAsync(posts.Select(c => c.AuthorId), ct);

        return CursorResponse.From(
            posts,
            limit,
            p =>
            {
                authors.TryGetValue(p.AuthorId, out var author);
                return new PostSummary(
                    p.Id.Value,
                    new PostAuthor(p.AuthorId.Value, author?.Username.Value ?? string.Empty),
                    p.Content.Value,
                    p.CreatedAt,
                    likeCounts.GetValueOrDefault(p.Id.Value),
                    commentCounts.GetValueOrDefault(p.Id.Value),
                    IsLiked: likedSet.Contains(p.Id.Value));
            },
            p => new Cursor(p.CreatedAt, p.Id.Value).Encode());
    }

    public async Task<PostDetail> GetByIdAsync(Guid postId, CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(PostId.From(postId), ct)
                   ?? throw new PostNotFoundException();

        var likesCount = await _postLikeRepository.GetCountAsync(post.Id, ct);
        var commentsCount = await _commentRepository.GetCountAsync(post.Id, ct);

        var author = await _authorRepository.GetByIdAsync(post.AuthorId, ct);

        return new PostDetail(post.Id.Value,
            new PostAuthor(post.AuthorId.Value, author?.Username.Value ?? string.Empty),
            post.Content.Value, post.CreatedAt, post.UpdatedAt,
            likesCount, commentsCount);
    }

    public async Task<Post> CreateAsync(Guid authorId, string content, CancellationToken ct = default)
    {
        var post = Post.Create(AuthorId.From(authorId), PostContent.From(content));
        _postRepository.Add(post);

        await _unitOfWork.CommitAsync(ct);

        return post;
    }

    public async Task<Post> UpdateAsync(Guid authorId, Guid postId, string content, CancellationToken ct = default)
    {
        var postIdVo = PostId.From(postId);
        var authorIdVo = AuthorId.From(authorId);
        var post = await _postRepository.GetByIdAsync(postIdVo, ct);

        if (post is null)
        {
            throw new PostNotFoundException();
        }

        post.Update(authorIdVo, PostContent.From(content));

        await _unitOfWork.CommitAsync(ct);

        return post;
    }

    public async Task DeleteAsync(Guid authorId, Guid postId, CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(PostId.From(postId), ct);

        if (post is null)
            throw new PostNotFoundException();

        post.Delete(AuthorId.From(authorId));

        await _unitOfWork.CommitAsync(ct);
    }

    public async Task LikeAsync(Guid authorId, Guid postId, CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(PostId.From(postId), ct)
                   ?? throw new PostNotFoundException();

        var existing = await _postLikeRepository.GetAsync(post.Id, AuthorId.From(authorId), ct);
        if (existing is not null) return;

        var like = PostLike.Create(post.Id, AuthorId.From(authorId));
        _postLikeRepository.Add(like);

        await _unitOfWork.CommitAsync(ct);
        await _postLikeRepository.RefreshCountsAsync(ct);
    }

    public async Task UnlikeAsync(Guid authorId, Guid postId, CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(PostId.From(postId), ct)
                   ?? throw new PostNotFoundException();

        var existing = await _postLikeRepository.GetAsync(post.Id, AuthorId.From(authorId), ct);
        if (existing is null) return;

        _postLikeRepository.Remove(existing);

        await _unitOfWork.CommitAsync(ct);
        await _postLikeRepository.RefreshCountsAsync(ct);
    }
}
