using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using AutoMapper;
using LMS.App.Services;

namespace LMS.App.AutoMapper
{
    public class AutoMapperConfig
    {
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(cfg =>
            {
                foreach(var p in LmsAppService.AutoMappersProfiles)
                    cfg.AddProfile(p);       
            });
        }
    }
}
