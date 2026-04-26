using FluentAssertions;
using Modules.Posts.Domain.Authors.ValueObjects;
using Modules.Posts.Domain.Follows;
using Modules.Posts.Domain.Follows.Events;
using SharedKernel.Domain.Exceptions;

namespace Modules.Posts.Domain.UnitTests;

public sealed class FollowTests
{
    // -- Create -----------------------------------------------

    [Fact]
    public void Create_Should_Return_Follow_With_Correct_Properties()
    {
        var followerId = new AuthorId(Guid.NewGuid());
        var followeeId = new AuthorId(Guid.NewGuid());

        var follow = Follow.Create(followerId, followeeId);

        follow.FollowerId.Should().Be(followerId);
        follow.FolloweeId.Should().Be(followeeId);
        follow.Id.Should().NotBeNull();
    }

    [Fact]
    public void Create_Should_Set_CreatedAt_To_UtcNow()
    {
        var before = DateTimeOffset.UtcNow;
        var follow = Follow.Create(new AuthorId(Guid.NewGuid()), new AuthorId(Guid.NewGuid()));
        var after = DateTimeOffset.UtcNow;

        follow.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Create_Should_Raise_AuthorFollowedDomainEvent()
    {
        var follow = Follow.Create(new AuthorId(Guid.NewGuid()), new AuthorId(Guid.NewGuid()));

        follow.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<AuthorFollowedDomainEvent>();
    }

    [Fact]
    public void Create_Should_Raise_Event_With_Correct_Ids()
    {
        var followerId = new AuthorId(Guid.NewGuid());
        var followeeId = new AuthorId(Guid.NewGuid());

        var follow = Follow.Create(followerId, followeeId);

        var evt = follow.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<AuthorFollowedDomainEvent>()
            .Subject;

        evt.FollowId.Should().Be(follow.Id.Value);
        evt.FollowerId.Should().Be(followerId.Value);
        evt.FolloweeId.Should().Be(followeeId.Value);
        evt.CreatedAt.Should().Be(follow.CreatedAt);
    }

    [Fact]
    public void Create_Should_Throw_When_FollowerId_Equals_FolloweeId()
    {
        var authorId = new AuthorId(Guid.NewGuid());

        var act = () => Follow.Create(authorId, authorId);

        act.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*cannot follow themselves*");
    }

    // -- Unfollow -----------------------------------------------

    [Fact]
    public void Unfollow_Should_Raise_AuthorUnfollowedDomainEvent()
    {
        var follow = Follow.Create(new AuthorId(Guid.NewGuid()), new AuthorId(Guid.NewGuid()));
        follow.ClearDomainEvents();

        follow.Unfollow();

        follow.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<AuthorUnfollowedDomainEvent>();
    }

    [Fact]
    public void Unfollow_Should_Raise_Event_With_Correct_Ids()
    {
        var followerId = new AuthorId(Guid.NewGuid());
        var followeeId = new AuthorId(Guid.NewGuid());
        var follow = Follow.Create(followerId, followeeId);
        follow.ClearDomainEvents();

        follow.Unfollow();

        var evt = follow.DomainEvents
            .Should().ContainSingle()
            .Which.Should().BeOfType<AuthorUnfollowedDomainEvent>()
            .Subject;

        evt.FollowId.Should().Be(follow.Id.Value);
        evt.FollowerId.Should().Be(followerId.Value);
        evt.FolloweeId.Should().Be(followeeId.Value);
    }
}
