using System;
using System.Collections.Generic;
using System.Text;

using LMS.Core.Interfaces;

namespace LMS.Core.Entities
{
    public class BaseEntity : IEntity
    {
        public int Id { get; set; }
    }
}
