using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.App.ViewModels
{
    public abstract class PersonViewModel : CrudViewModel
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
