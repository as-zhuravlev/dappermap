using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Reports
{
    public class ReportArgument
    { 
        public string Name { get; }

        public Type Type { get; }

        public ReportArgument(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
