using System;
using System.Collections.Generic;
using System.Text;

using LMS.Core.Interfaces;

namespace LMS.Core.Notifications
{
    public abstract class Notification : IEntity
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        public string Message { get; set; }

        public bool IsSent { get; set; }
    }
}
