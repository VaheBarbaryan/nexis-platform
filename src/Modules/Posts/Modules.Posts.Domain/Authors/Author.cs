using Modules.Posts.Domain.Authors.ValueObjects;
using SharedKernel.Domain.Entities;

namespace Modules.Posts.Domain.Authors;

public sealed class Author : Entity<AuthorId>
{
    public Username Username { get; private set; } = null!;

    private Author()
    {
    }

    private Author(AuthorId id, Username username)
    {
        Id = id;
        Username = username;
    }

    public static Author Create(AuthorId id, Username username)
    {
        return new Author(id, username);
    }
}
