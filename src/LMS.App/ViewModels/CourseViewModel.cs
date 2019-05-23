using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.App.ViewModels
{
    public class CourseViewModel : CrudViewModel
    {
        public string Name { get; set; }

        public int LectorId { get; set; }
    }
}
