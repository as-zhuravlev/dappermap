using System;

namespace LMS.Core.Entities
{
    public class Mark : BaseEntity
    {   
        public int StudentCourseId { get; set; }

        public int LectionId { get; set; }

        public short Value { get; set; }

        public static short AbsenceValue => -1;
    }
}
