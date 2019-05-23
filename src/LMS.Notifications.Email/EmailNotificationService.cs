using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using LMS.Core.Interfaces;


namespace LMS.Notifications.Email
{
    public class EmailNotificationService : INotificationService
    {
        private readonly INotificationProvider _provider;

        public EmailNotificationService(INotificationProvider provider)
        {
            _provider = provider;
        }

        public void SendNotifacations()
        {
           IReadOnlyCollection<EmailNotification> messages = _provider.GetUnSentNotifications<EmailNotification>();

            using (var smsLog = new StreamWriter("email.log", true))
            {
                foreach (var m in messages)
                {
                    m.IsSent = true;
                    smsLog.WriteLine($"To: {m.PersonId}, Phone: {m.Email}, Text : {m.Message}");
                }
            }

            _provider.MarkNotificationsAsSent(messages);
        }
    }
}
