using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

using LMS.App.ViewModels;

namespace LMS.App.Validation
{
    public class PersonValidator<T> : AbstractValidator<T> where T: PersonViewModel
    {
        public PersonValidator()
        {
            RuleFor(person => person.Name)
                .NotEmpty()
                .MaximumLength(30)
                .Matches(@"^([\p{L}]+ )+[\p{L}]+$|^[\p{L}]+$").WithMessage("Only alphabetic words separating with single spaces are allowed in name");

            RuleFor(person => person.Email)
               .EmailAddress();

            RuleFor(person => person.Phone)
                .Matches(@"^\+\d{8,15}$").WithMessage("Use international telephone format: plus sign, contry code, and digits without any another symbols");
        }
    }
}
