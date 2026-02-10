using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Application.Contracts;
using SharedKernel.Infrastructure.EventBus;
using SharedKernel.Infrastructure.Messaging;
using SharedKernel.Infrastructure.Messaging.Commands;

namespace Modules.Users.Application.Notifications.Handlers;

public class SendPasswordResetEmailHandler : INotificationHandler<PasswordResetRequestedNotification>
{
    private readonly IEventBusPublisher _eventBusPublisher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ITokenStore<Guid> _passwordTokenStore;
    private readonly IPasswordResetLinkBuilder _passwordResetLinkBuilder;

    public SendPasswordResetEmailHandler(
        IEventBusPublisher eventBusPublisher,
        ITokenGenerator tokenGenerator,
        [FromKeyedServices(TokenStoreKey.PasswordReset)]
        ITokenStore<Guid> passwordTokenStore,
        IPasswordResetLinkBuilder passwordResetLinkBuilder)
    {
        _eventBusPublisher = eventBusPublisher;
        _tokenGenerator = tokenGenerator;
        _passwordTokenStore = passwordTokenStore;
        _passwordResetLinkBuilder = passwordResetLinkBuilder;
    }

    public async Task Handle(PasswordResetRequestedNotification notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var token = _tokenGenerator.Generate();
        var hashedToken = _tokenGenerator.Hash(token);

        await _passwordTokenStore.StoreAsync(
            notification.Id,
            hashedToken,
            TimeSpan.FromHours(24),
            cancellationToken);

        var resetUri = _passwordResetLinkBuilder.Build(token);

        var emailCommand = new SendEmailIntegrationCommand
        {
            Template = EmailTemplates.PasswordReset,
            To = notification.DomainEvent.Email,
            Subject = "Password Reset",
            Language = "en",
            Variables = new Dictionary<string, string>
            {
                ["Username"] = notification.DomainEvent.Username,
                ["ResetUrl"] = resetUri.AbsoluteUri,
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
