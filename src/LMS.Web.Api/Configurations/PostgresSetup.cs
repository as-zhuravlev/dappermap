using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using LMS.Postgres.Configuration;
using LMS.Core.Interfaces;
using LMS.Postgres;
using Microsoft.Extensions.Configuration;

namespace LMS.Web.Api.Configurations
{
    public static class PostgresSetup
    {
        public static void AddPgRepository(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddScoped<IRepository, PgRepository>();
            
            PgConfig.CreateRelations(PgRepositoryConf.GetConnectionString(configuration));
        }

        public static void SeedTestData(IConfiguration configuration)
        {
            PgConfig.SeedData(PgRepositoryConf.GetConnectionString(configuration));
        }
    }
}
