using System;

using LMS.Core.Notifications;

namespace LMS.Notifications.SMS
{
    public class SmsNotification : Notification
    {
        public string Phone { get; set; }
    }
}
