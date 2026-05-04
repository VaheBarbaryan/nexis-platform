using Modules.Notifications.Application.Contracts;
using Modules.Notifications.Domain;
using Modules.Notifications.Domain.NotificationActors;
using Modules.Notifications.Domain.NotificationActors.Repositories;
using Modules.Notifications.Domain.NotificationActors.ValueObjects;

namespace Modules.Notifications.Application.Services;

public sealed class NotificationActorService : INotificationActorService
{
    private readonly INotificationActorRepository _notificationActorRepository;
    private readonly INotificationsUnitOfWork _unitOfWork;

    public NotificationActorService(
        INotificationActorRepository notificationActorRepository,
        INotificationsUnitOfWork unitOfWork)
    {
        _notificationActorRepository = notificationActorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<NotificationActor> CreateAsync(Guid userId, string username, CancellationToken ct)
    {
        var id = new NotificationActorId(userId);

        if (await _notificationActorRepository.ExistsAsync(id, ct))
            return NotificationActor.Create(userId, username);

        var notificationActor = NotificationActor.Create(userId, username);
        _notificationActorRepository.Add(notificationActor);

        await _unitOfWork.CommitAsync(ct);
        return notificationActor;
    }
}
