using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Npgsql;

using DapperMap;
using LMS.Core.Interfaces;
using LMS.Core.Notifications;
using LMS.Notifications.Email;
using LMS.Notifications.SMS;

namespace LMS.Postgres
{
    public class PgNotifactionProvider : BaseRepository, INotificationProvider
    {
        public PgNotifactionProvider(IConfiguration conf) : base(NpgsqlFactory.Instance, PgRepositoryConf.GetConnectionString(conf)) { }

        public IReadOnlyCollection<TN> GetUnSentNotifications<TN>() where TN : Notification, new()
        {
            return this.List<TN>(x => x.IsSent == false);
        }

        public void MarkNotificationsAsSent<TN>(IReadOnlyCollection<TN> notifications) where TN : Notification, new()
        {
            foreach (var n in notifications)
                n.IsSent = true;

            this.Update(notifications);
            
        }

        protected override void ConfigureMapping()
        {
            AddMapping<SmsNotification>(new PgTableMapping<SmsNotification>("SmsMessages"));
            AddMapping<EmailNotification>(new PgTableMapping<EmailNotification>("EmailLetters"));
        }
    }
}
