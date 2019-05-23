using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Entities
{
    public class StudentCourse : BaseEntity
    {
        public int StudentId { get; set; }

        public int CourseId { get; set; }
    }
}
