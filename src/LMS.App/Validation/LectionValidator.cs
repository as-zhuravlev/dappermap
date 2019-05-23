using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

using LMS.App.ViewModels;


namespace LMS.App.Validation
{
    public class LectionValidator : AbstractValidator<LectionViewModel>
    {
        public LectionValidator()
        {
            RuleFor(c => c.Name).NotEmpty().Length(1, 30);
        }
    }
}
