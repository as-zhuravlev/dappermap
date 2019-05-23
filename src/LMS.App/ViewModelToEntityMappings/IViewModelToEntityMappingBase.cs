using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace LMS.App.ViewModelToEntityMappings
{
    public interface IViewModelToEntityMappingBase
    {
        IEnumerable<Profile> AutoMappersProfiles { get; }
    }
}
