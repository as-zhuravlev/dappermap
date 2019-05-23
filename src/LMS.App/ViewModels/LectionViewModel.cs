using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.App.ViewModels
{
    public class LectionViewModel : CrudViewModel
    {
        public string Name { get; set; }

        public int CourseId { get; set; }

        public DateTime Date { get; set; }
    }
}
