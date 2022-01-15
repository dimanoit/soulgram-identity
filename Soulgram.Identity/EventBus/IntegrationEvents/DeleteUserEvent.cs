using Soulgram.Eventbus;

namespace Soulgram.Identity.IntegrationEvents
{
	public class DeleteUserEvent: IntegrationEvent
	{
		public DeleteUserEvent(string userId)
		{
			UserId = userId;
		}

		public string UserId { get; }
	}
}