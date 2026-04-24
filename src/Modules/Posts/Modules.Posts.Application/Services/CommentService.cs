using Modules.Posts.Application.Contracts;
using Modules.Posts.Domain;
using Modules.Posts.Domain.Authors.Repositories;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Comments;
using Modules.Posts.Domain.Comments.Exceptions;
using Modules.Posts.Domain.Comments.Repositories;
using Modules.Posts.Domain.Comments.ValueObjects;
using Modules.Posts.Domain.Posts.Exceptions;
using Modules.Posts.Domain.Posts.Repositories;
using Modules.Posts.Domain.Posts.ValueObjects;
using SharedKernel.Application.Pagination;

namespace Modules.Posts.Application.Services;

public sealed class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IPostUnitOfWork _unitOfWork;

    public CommentService(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        IAuthorRepository authorRepository,
        IPostUnitOfWork unitOfWork)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _authorRepository = authorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CursorResponse<CommentSummary>> GetByPostIdAsync(
        Guid postId, string? cursor, int limit = 20, CancellationToken ct = default)
    {
        var comments = await _commentRepository.GetByPostIdAsync(new PostId(postId), cursor, limit, ct);

        var authors = await _authorRepository.GetByIdsAsync(comments.Select(c => c.AuthorId), ct);

        return CursorResponse.From(
            comments,
            limit,
            c =>
            {
                authors.TryGetValue(c.AuthorId, out var author);
                return new CommentSummary(
                    c.Id.Value,
                    c.PostId.Value,
                    new CommentAuthor(c.AuthorId.Value, author?.Username ?? string.Empty),
                    c.Content,
                    c.CreatedAt,
                    c.UpdatedAt);
            },
            c => new Cursor(c.CreatedAt, c.Id.Value).Encode());
    }

    public async Task<CommentSummary> CreateAsync(Guid authorId, Guid postId, string content, CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(new PostId(postId), ct)
            ?? throw new PostNotFoundException();

        var comment = Comment.Create(post.Id, new AuthorId(authorId), content);
        _commentRepository.Add(comment);

        await _unitOfWork.CommitAsync(ct);

        var author = await _authorRepository.GetByIdAsync(comment.AuthorId, ct);

        return new CommentSummary(
            comment.Id.Value,
            comment.PostId.Value,
            new CommentAuthor(comment.AuthorId.Value, author?.Username ?? string.Empty),
            comment.Content,
            comment.CreatedAt,
            comment.UpdatedAt);
    }

    public async Task<CommentSummary> UpdateAsync(Guid authorId, Guid commentId, string content, CancellationToken ct = default)
    {
        var comment = await _commentRepository.GetByIdAsync(new CommentId(commentId), ct)
            ?? throw new CommentNotFoundException();

        comment.Update(new AuthorId(authorId), content);

        await _unitOfWork.CommitAsync(ct);

        var author = await _authorRepository.GetByIdAsync(comment.AuthorId, ct);

        return new CommentSummary(
            comment.Id.Value,
            comment.PostId.Value,
            new CommentAuthor(comment.AuthorId.Value, author?.Username ?? string.Empty),
            comment.Content,
            comment.CreatedAt,
            comment.UpdatedAt);
    }

    public async Task DeleteAsync(Guid authorId, Guid commentId, CancellationToken ct = default)
    {
        var comment = await _commentRepository.GetByIdAsync(new CommentId(commentId), ct)
            ?? throw new CommentNotFoundException();

        comment.Delete(new AuthorId(authorId));

        await _unitOfWork.CommitAsync(ct);
    }
}
