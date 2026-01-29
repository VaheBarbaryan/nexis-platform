using MediatR;
using Modules.Users.Application.Contracts;
using Modules.Users.IntegrationEvents;

namespace Modules.Users.Application.Notifications;

public class UserCreatedNotificationHandler : INotificationHandler<UserCreatedNotification>
{
    private readonly IEmailVerificationTokenStore _emailVerificationTokenStore;
    private readonly IVerificationLinkBuilder _verificationLinkBuilder;
    private readonly ITokenGenerator _tokenGenerator;

    public UserCreatedNotificationHandler(
        IEmailVerificationTokenStore emailVerificationTokenStore,
        IVerificationLinkBuilder verificationLinkBuilder,
        ITokenGenerator tokenGenerator)
    {
        _emailVerificationTokenStore = emailVerificationTokenStore;
        _verificationLinkBuilder = verificationLinkBuilder;
        _tokenGenerator = tokenGenerator;
    }

    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var token = _tokenGenerator.Generate();
        var tokenHash = _tokenGenerator.Hash(token);

        await _emailVerificationTokenStore.StoreAsync(
            notification.DomainEvent.UserId,
            tokenHash,
            TimeSpan.FromHours(24),
            cancellationToken);

        var verificationUri = _verificationLinkBuilder.Build(token);

        var integrationEvent = new UserEmailVerificationRequestedIntegrationEvent(
            notification.DomainEvent.UserId,
            notification.DomainEvent.Email,
            verificationUri.AbsoluteUri);

        Console.WriteLine(integrationEvent);
    }
}
