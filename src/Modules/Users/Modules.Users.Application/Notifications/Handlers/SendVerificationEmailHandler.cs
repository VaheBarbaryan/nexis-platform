using MediatR;
using Modules.Users.Application.Contracts;
using SharedKernel.Infrastructure.EventBus;
using SharedKernel.Infrastructure.Messaging;
using SharedKernel.Infrastructure.Messaging.Commands;

namespace Modules.Users.Application.Notifications.Handlers;

public class SendVerificationEmailHandler : INotificationHandler<EmailVerificationRequestedNotification>
{
    private readonly IEventBusPublisher _eventBusPublisher;
    private readonly IEmailVerificationTokenStore _emailVerificationTokenStore;
    private readonly IVerificationLinkBuilder _verificationLinkBuilder;
    private readonly ITokenGenerator _tokenGenerator;

    public SendVerificationEmailHandler(
        IEventBusPublisher eventBusPublisher,
        IEmailVerificationTokenStore emailVerificationTokenStore,
        IVerificationLinkBuilder verificationLinkBuilder,
        ITokenGenerator tokenGenerator)
    {
        _eventBusPublisher = eventBusPublisher;
        _emailVerificationTokenStore = emailVerificationTokenStore;
        _verificationLinkBuilder = verificationLinkBuilder;
        _tokenGenerator = tokenGenerator;
    }

    public async Task Handle(EmailVerificationRequestedNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Send Verification Email Handler {notification}");

        ArgumentNullException.ThrowIfNull(notification);

        var token = _tokenGenerator.Generate();
        var tokenHash = _tokenGenerator.Hash(token);

        await _emailVerificationTokenStore.StoreAsync(
            notification.DomainEvent.UserId,
            tokenHash,
            TimeSpan.FromHours(24),
            cancellationToken);

        var verificationUri = _verificationLinkBuilder.Build(token);

        var emailCommand = new SendEmailIntegrationCommand
        {
            Template = EmailTemplates.VerifyEmail,
            To = notification.DomainEvent.Email,
            Subject = "Email Verification",
            Language = "en",
            Variables = new Dictionary<string, string>
            {
                ["Username"] = notification.DomainEvent.Username,
                ["VerificationUrl"] = verificationUri.AbsoluteUri,
                ["ExpiryHours"] = "24"
            }
        };

        await _eventBusPublisher.PublishAsync(
            KafkaTopics.NotificationsEmailV1,
            emailCommand,
            partitionKey: notification.DomainEvent.UserId.ToString()
        );
    }
}
