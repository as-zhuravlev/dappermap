using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using LMS.Core.Interfaces;


namespace LMS.Notifications.SMS
{
    public class SmsNotificationService : INotificationService
    {
        private readonly INotificationProvider _provider;

        public SmsNotificationService(INotificationProvider provider)
        {
            _provider = provider;
        }

        public void SendNotifacations()
        {
            IReadOnlyCollection<SmsNotification> messages = _provider.GetUnSentNotifications<SmsNotification>();

            using (var smsLog = new StreamWriter("sms.log", true))
            {
                foreach (var m in messages)
                {
                    m.IsSent = true;
                    smsLog.WriteLine($"To: {m.PersonId}, Phone: {m.Phone}, Text : {m.Message}");
                }
            }

            _provider.MarkNotificationsAsSent(messages);
        }
    }
}
