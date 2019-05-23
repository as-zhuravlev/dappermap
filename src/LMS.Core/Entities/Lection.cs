using System;

namespace LMS.Core.Entities
{
    public class Lection : BaseEntity
    {
        public string Name { get; set; }

        public int CourseId { get; set; }

        public DateTime Date { get; set; }
    }
}
