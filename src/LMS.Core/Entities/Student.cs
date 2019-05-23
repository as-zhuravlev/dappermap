using System;
using System.Collections.Generic;
using System.Text;

using LMS.Core.Interfaces;

namespace LMS.Core.Entities
{
    public class Student : Person, IEntity
    {
        public int Id { get; set; }
    }
}
