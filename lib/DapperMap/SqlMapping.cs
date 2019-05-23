using System;
using System.Collections.Generic;
using System.Text;

namespace DapperMap
{
    public class SqlMapping : IMapping
    {
        public SqlMapping(string selectFromRelation,
                                     string createSql,
                                     string updateSql,
                                     string deleteSql)
        {
            SelectFromRelation = selectFromRelation;
            CreateSql = createSql;
            UpdateSql = updateSql;
            DeleteSql = deleteSql;
        }

        public string SelectFromRelation { get; }

        public string CreateSql { get; }

        public string UpdateSql { get; }

        public string DeleteSql { get; }

    }
}
