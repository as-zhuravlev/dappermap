using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

using LMS.App.ViewModels;

namespace LMS.App.Validation
{
    public class CourseValidator : AbstractValidator<CourseViewModel>
    {
        public CourseValidator()
        {
            RuleFor(c => c.Name).NotEmpty().Length(1, 30);     
        }
    }
}
