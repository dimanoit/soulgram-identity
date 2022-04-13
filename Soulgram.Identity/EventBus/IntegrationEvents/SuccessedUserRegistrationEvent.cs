﻿using System;
using Soulgram.Eventbus;

namespace Soulgram.Identity.IntegrationEvents;

public class SuccessedUserRegistrationEvent : IntegrationEvent
{
    public SuccessedUserRegistrationEvent(
        string nickname,
        string email,
        string userId,
        DateTime birthday)
    {
        Nickname = nickname;
        Email = email;
        UserId = userId;
        Birthday = birthday;
    }

    public string Nickname { get; }
    public string Email { get; }
    public DateTime Birthday { get; }
    public string UserId { get; }
}