using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Data.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;

using FluentValidation.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;
using AutoMapper;

using DapperMap;
using LMS.Postgres;
using LMS.Notifications.Email;
using LMS.Notifications.SMS;
using LMS.Core.Interfaces;
using LMS.App.Interfaces;
using LMS.App.Services;
using LMS.App.Validation;
using LMS.Web.Api.Configurations;
using LMS.Web.Api.ActionFilters;

namespace LMS.Web
{
    public class Startup
    {
        private readonly ILogger _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateViewModelStateAttribute));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
              .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<StudentValidator>());

            services.AddAutoMapperService();
            _logger.LogInformation("Added AutoMapper to services");

            services.AddPgRepository(Configuration);
            _logger.LogInformation("Added PgDbRepository to services");

            services.AddScoped<INotificationProvider, PgNotifactionProvider>();
            _logger.LogInformation("Added PgNotifactionProvider to services");
            
            services.AddScoped<INotificationService, SmsNotificationService>();
            _logger.LogInformation("Added  SmsNotificationService to services");

            services.AddScoped<INotificationService, EmailNotificationService>();
            _logger.LogInformation("Added EmailNotificationService to services");

            services.AddScoped<ILmsAppService, LmsAppService>();
            _logger.LogInformation("Added LmsAppService to services");
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "LMS API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

            });
            _logger.LogInformation("Added Swagger to services");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMapper mapper)
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                PostgresSetup.SeedTestData(Configuration);
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            var cachePeriod = env.IsDevelopment() ? "600" : "604800";
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append(new KeyValuePair<string, StringValues>("Cache-Control", $"public, max-age={cachePeriod}"));
                }
            });

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LMS API V1");
            });

            app.UseHttpsRedirection();
            
            app.UseMvc();
        }
    }
}
