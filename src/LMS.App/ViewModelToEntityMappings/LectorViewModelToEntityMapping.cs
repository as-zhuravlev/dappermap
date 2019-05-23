using System;
using System.Collections.Generic;
using System.Text;

using AutoMapper;

using LMS.Core.Entities;
using LMS.App.ViewModels;

namespace LMS.App.ViewModelToEntityMappings
{
    class LectorViewModelToEntityMapping : ViewModelToEntityMapping<LectorViewModel, Lector>
    {
        public override IEnumerable<Profile> AutoMappersProfiles => new Profile[] { new EntityModelViewProfile() };

        class EntityModelViewProfile : Profile
        {
            public EntityModelViewProfile()
            {
                CreateMap<LectorViewModel, Lector>().ForMember(x => x.PersonId, opt => opt.Ignore());
                CreateMap<Lector,LectorViewModel>().ForSourceMember(x => x.PersonId, opt => opt.DoNotValidate());
            }
        }
    }
}
