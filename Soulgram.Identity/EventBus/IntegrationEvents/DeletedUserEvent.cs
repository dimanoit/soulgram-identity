using Soulgram.Eventbus;

namespace Soulgram.Identity.IntegrationEvents;

public class DeletedUserEvent : IntegrationEvent
{
    public DeletedUserEvent(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; }
}