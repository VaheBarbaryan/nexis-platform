using Microsoft.EntityFrameworkCore;
using Modules.Posts.Domain.Posts;
using Modules.Posts.Domain.Posts.Repositories;
using Modules.Posts.Domain.Posts.ValueObjects;
using Modules.Posts.Persistence.Contexts;

namespace Modules.Posts.Persistence.Repositories;

public sealed class PostRepository : IPostRepository
{
    private readonly PostsDbContext _context;

    public PostRepository(PostsDbContext context)
    {
        _context = context;
    }

    public async Task<Post?> GetByIdAsync(PostId postId, CancellationToken ct = default)
    {
        return await _context.Posts.SingleOrDefaultAsync(x => x.Id == postId, cancellationToken: ct);
    }

    public void Add(Post post)
    {
        _context.Posts.Add(post);
    }
}
