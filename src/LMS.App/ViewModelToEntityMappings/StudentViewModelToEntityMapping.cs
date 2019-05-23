using System;
using System.Collections.Generic;
using System.Text;

using AutoMapper;

using LMS.Core.Entities;
using LMS.App.ViewModels;

namespace LMS.App.ViewModelToEntityMappings
{
    class StudentViewModelToEntityMapping : ViewModelToEntityMapping<StudentViewModel, Student>
    {
        public override IEnumerable<Profile> AutoMappersProfiles => new Profile[] { new EntityModelViewProfile() };

        class EntityModelViewProfile : Profile
        {
            public EntityModelViewProfile()
            {
                CreateMap<StudentViewModel, Student>().ForMember(x => x.PersonId, opt => opt.Ignore());
                CreateMap<Student, StudentViewModel>().ForSourceMember(x => x.PersonId, opt => opt.DoNotValidate());
            }
        }
    }
}
