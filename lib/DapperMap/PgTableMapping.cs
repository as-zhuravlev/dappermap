using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;


namespace DapperMap
{
    public class PgTableMapping<TEntity> : IMapping where TEntity : new()
    {
        private static readonly string _createSqlformat;
        private static readonly string _updateSqlformat;
        private static readonly string _deleteSqlformat;

        public PgTableMapping(string table)
        {
            SelectFromRelation = table;
            CreateSql = string.Format(_createSqlformat, table);
            UpdateSql = string.Format(_updateSqlformat, table);
            DeleteSql = string.Format(_deleteSqlformat, table);
        }

        static PgTableMapping()
        {

            var sbCreate = new StringBuilder();
            sbCreate.Append("INSERT INTO {0} (");

            var sbUpdate = new StringBuilder();
            sbUpdate.Append("UPDATE {0} SET ");

            var props = typeof(TEntity).GetProperties(BindingFlags.Instance |
                                                   BindingFlags.Public |
                                                   BindingFlags.SetProperty |
                                                   BindingFlags.GetProperty);
            bool foundIdProp = false; 

            foreach (var pi in props)
            {
                if (pi.Name == "Id")
                {
                    foundIdProp = true;
                    continue;
                }

                sbCreate.Append(pi.Name);
                sbCreate.Append(",");

                sbUpdate.Append(pi.Name);
                sbUpdate.Append(" = @");
                sbUpdate.Append(pi.Name);
                sbUpdate.Append(",");
            }

            if (!foundIdProp)
            {
                throw new ArgumentException("Can not find id Property in Type.", typeof(TEntity).Name);
            }

            sbCreate[sbCreate.Length - 1] = ')';
            sbCreate.Append(" VALUES (");

            sbUpdate[sbUpdate.Length - 1] = ' ';

            foreach (var pi in props)
            {
                if (pi.Name == "Id")
                    continue;

                sbCreate.Append("@");
                sbCreate.Append(pi.Name);
                sbCreate.Append(",");
            }
            sbCreate[sbCreate.Length - 1] = ')';

            sbCreate.Append(" RETURNING Id");

            _createSqlformat = sbCreate.ToString();

            sbUpdate.Append("WHERE Id = @Id");

            _updateSqlformat = sbUpdate.ToString();
            
            _deleteSqlformat = "DELETE FROM {0} WHERE Id = @Id";

        }

        public string SelectFromRelation { get; }

        public string CreateSql { get; }
        
        public string UpdateSql { get; }
    
        public string DeleteSql { get; }
    }
}
