using System;
using System.Collections.Generic;
using System.Text;

namespace DapperMap
{
    public interface IMapping
    {
        string SelectFromRelation { get; }
        string CreateSql { get; }
        string UpdateSql { get; }
        string DeleteSql { get; }
    }
}
