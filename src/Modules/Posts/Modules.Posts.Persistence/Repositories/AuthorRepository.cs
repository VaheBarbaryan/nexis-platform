using Microsoft.EntityFrameworkCore;
using Modules.Posts.Domain.Authors;
using Modules.Posts.Domain.Authors.Repositories;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Persistence.Contexts;

namespace Modules.Posts.Persistence.Repositories;

public sealed class AuthorRepository : IAuthorRepository
{
    private readonly PostsDbContext _context;

    public AuthorRepository(PostsDbContext context)
    {
        _context = context;
    }

    public async Task<Author?> GetByIdAsync(AuthorId authorId, CancellationToken ct = default)
    {
        return await _context.Authors.SingleOrDefaultAsync(a => a.Id == authorId, ct);
    }

    public async Task<Dictionary<AuthorId, Author>> GetByIdsAsync(
        IEnumerable<AuthorId> authorIds, CancellationToken ct = default)
    {
        return await _context.Authors
            .Where(a => authorIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, ct);
    }

    public void Add(Author author)
    {
        _context.Authors.Add(author);
    }
}
