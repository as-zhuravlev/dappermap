using System.Data;
using Dapper;

namespace DapperMap
{
    public interface ISelection
    {
        IDbConnection DbConnection { get; }

        string[] Relations { get; }

        string SelectSql { get; }

        string JoinOnSql { get; }

        string WhereSql { get; }

        DynamicParameters DynamicParameters { get; }
    }

    public interface ISelection<T> : ISelection
    {
    }

    public interface ISelection<T1, T2> : ISelection
    {
    }

    public interface ISelection<T1, T2, T3> : ISelection
    {
    }

    public interface ISelection<T1, T2, T3, T4> : ISelection
    {
    }

    public interface ISelection<T1, T2, T3, T4, T5> : ISelection
    {
    }
}
