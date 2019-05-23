using System;
using System.Collections.Generic;
using System.Text;

using LMS.Core.Notifications;

namespace LMS.Core.Interfaces
{
    public interface INotificationProvider
    {
        IReadOnlyCollection<TN> GetUnSentNotifications<TN>() where TN : Notification, new();
        void MarkNotificationsAsSent<TN>( IReadOnlyCollection<TN> notifications) where TN : Notification, new();
    }
}
