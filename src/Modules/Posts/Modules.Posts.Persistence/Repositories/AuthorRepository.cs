using Modules.Posts.Domain.Authors;
using Modules.Posts.Domain.Authors.Repositories;
using Modules.Posts.Persistence.Contexts;

namespace Modules.Posts.Persistence.Repositories;

public sealed class AuthorRepository : IAuthorRepository
{
    private readonly PostsDbContext _context;

    public AuthorRepository(PostsDbContext context)
    {
        _context = context;
    }

    public void Add(Author author)
    {
        _context.Authors.Add(author);
    }
}
