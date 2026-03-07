using Modules.Posts.Domain.Authors.ValueObjects;
using SharedKernel.Domain.Entities;

namespace Modules.Posts.Domain.Authors;

public class Author : Entity<AuthorId>
{
    public string Username { get; private set; } = null!;

    private Author()
    {
    }

    private Author(string username)
    {
        Id = new AuthorId(Guid.NewGuid());
        Username = username;
    }

    public static Author Create(string username)
    {
        var author = new Author(username);

        return author;
    }
}
