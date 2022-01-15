using Soulgram.Eventbus;

namespace Soulgram.Identity.IntegrationEvents
{
	public class SuccessUserRegistrationEvent : IntegrationEvent
	{
		public SuccessUserRegistrationEvent(string userName, string email, string userId)
		{
			UserName = userName;
			Email = email;
			UserId = userId;
		}

		public string UserName { get; }
		public string Email { get; }
		public string UserId { get; }
	}
}
