using Modules.Posts.Domain.Authors.ValueObjects;
using SharedKernel.Domain.Entities;

namespace Modules.Posts.Domain.Authors;

public class Author : Entity<AuthorId>
{
    public string Username { get; private set; } = null!;

    private Author()
    {
    }

    private Author(AuthorId id, string username)
    {
        Id = id;
        Username = username;
    }

    public static Author Create(Guid id, string username)
    {
        return new Author(new AuthorId(id), username);
    }
}
