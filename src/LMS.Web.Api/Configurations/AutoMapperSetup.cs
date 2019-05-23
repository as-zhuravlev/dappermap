using System;
using AutoMapper;
using LMS.App.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Web.Api.Configurations
{
    public static class AutoMapperSetup
    {
        public static void AddAutoMapperService(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            
            IMapper mapper = AutoMapperConfig.RegisterMappings().CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}