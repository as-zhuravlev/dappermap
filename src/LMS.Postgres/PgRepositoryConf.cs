using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace LMS.Postgres
{
    public static class PgRepositoryConf
    {
        private const string DatabaseKey = "Database";
        private const string PgRepositoryKey = "PgRepository";

        public static string GetConnectionString(IConfiguration conf)
        {
            var sec = conf.GetSection(PgRepositoryKey);
            var db = sec.GetValue<string>(DatabaseKey);
            return conf.GetConnectionString(db);
        }

    }
}
