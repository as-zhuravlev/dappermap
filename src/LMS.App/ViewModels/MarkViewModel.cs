using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.App.ViewModels
{
    public class MarkViewModel
    {
        public int Id { get; set; }

        public StudentViewModel Student{ get; set; }
        
        public CourseViewModel Course{ get; set; }

        public LectionViewModel Lection { get; set; }
        
        public string Value { get; set; }
    }
}
