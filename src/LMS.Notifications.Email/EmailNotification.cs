using System;

using LMS.Core.Notifications;

namespace LMS.Notifications.Email
{
    public class EmailNotification : Notification
    {
        public string Email { get; set; }
    }
}
