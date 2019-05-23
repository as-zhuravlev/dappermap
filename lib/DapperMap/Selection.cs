using System;
using System.Linq;
using System.Data;
using System.Linq.Expressions;

using Dapper;

namespace DapperMap
{
    internal class Selection
    {
        protected Selection(IDbConnection dbConnection)
        {
            DbConnection = dbConnection;
        }

        Selection() { }

        public IDbConnection DbConnection { get; protected set; }

        public string[] Relations { get; protected set; }

        public string SelectSql
        {
            get
            {
                return "SELECT * FROM " + Relations.Aggregate((acc, r) => acc + ", " + r);
            }
        }

        public string JoinOnSql { get; protected set; }

        public string WhereSql { get; protected set; }

        public DynamicParameters DynamicParameters { get; protected set; } = new DynamicParameters();

        protected static void CopySelection<TSelection>(ISelection src, TSelection dst) where TSelection : Selection
        {
            dst.Relations = src.Relations.ToArray();
            dst.JoinOnSql = src.JoinOnSql;
            dst.WhereSql = src.WhereSql;
            dst.DynamicParameters = src.DynamicParameters;
        }

        public static string ConcatSqlConditions(string left, string right)
        {
            if (string.IsNullOrWhiteSpace(left))
                return right;

            if (string.IsNullOrWhiteSpace(right))
                return left;

            return left + " AND " + right;
        }
    }

    internal class Selection<T> : Selection, ISelection<T>
    {
        public Selection(IDbConnection dbConnection, string relation) : base(dbConnection)
        {
            Relations = new[] { relation };
        }

        public static Selection<T> AddWhereCondition(ISelection<T> selection, Expression<Predicate<T>> predicate)
        {
            Selection<T> res = new Selection<T>(selection.DbConnection, selection.Relations[0]);
            CopySelection(selection, res);
            res.WhereSql = ConcatSqlConditions(selection.WhereSql,
                                               PredicateToSqlCondition.Convert(predicate,
                                                                               selection.DynamicParameters,
                                                                               selection.Relations[0]));
            return res;
        }
    }

    internal class Selection<T1, T2> : Selection, ISelection<T1, T2>
    {
        public Selection(IDbConnection dbConnection, string relation1, string relation2) : base(dbConnection)
        {
            Relations = new[] { relation1, relation2 };
        }

        public static Selection<T1, T2> AddWhereCondition(ISelection<T1, T2> selection, Expression<Func<T1, T2, bool>> predicate)
        {
            Selection<T1, T2> res = new Selection<T1, T2>(selection.DbConnection, selection.Relations[0], selection.Relations[1]);
            CopySelection(selection, res);
            res.WhereSql = ConcatSqlConditions(selection.WhereSql,
                                               PredicateToSqlCondition.Convert(predicate,
                                                                               selection.DynamicParameters,
                                                                               selection.Relations[0], selection.Relations[1]));
            return res;
        }

        public static Selection<T1, T2> AddJoinOnCondition(ISelection<T1, T2> selection, Expression<Func<T1, T2, bool>> predicate)
        {
            Selection<T1, T2> res = new Selection<T1, T2>(selection.DbConnection, selection.Relations[0], selection.Relations[1]);
            CopySelection(selection, res);
            res.JoinOnSql = ConcatSqlConditions(selection.JoinOnSql,
                                               PredicateToSqlCondition.Convert(predicate,
                                                                               selection.DynamicParameters,
                                                                               selection.Relations[0], selection.Relations[1]));
            return res;
        }

    }

    internal class Selection<T1, T2, T3> : Selection, ISelection<T1, T2, T3>
    {
        public Selection(IDbConnection dbConnection, string relation1, string relation2, string relation3) : base(dbConnection)
        {
            Relations = new[] { relation1, relation2, relation3 };
        }

        public static Selection<T1, T2, T3> AddWhereCondition(ISelection<T1, T2, T3> selection, Expression<Func<T1, T2, T3, bool>> predicate)
        {
            Selection<T1, T2, T3> res = new Selection<T1, T2, T3>(selection.DbConnection,
                                                              selection.Relations[0],
                                                              selection.Relations[1],
                                                              selection.Relations[2]);
            CopySelection(selection, res);
            res.WhereSql = ConcatSqlConditions(selection.WhereSql,
                                               PredicateToSqlCondition.Convert(predicate,
                                                                               selection.DynamicParameters,
                                                                               selection.Relations[0],
                                                                               selection.Relations[1],
                                                                               selection.Relations[2]));
            return res;
        }

        public static Selection<T1, T2, T3> AddJoinOnCondition(ISelection<T1, T2, T3> selection, Expression<Func<T1, T2, T3, bool>> predicate)
        {
            Selection<T1, T2, T3> res = new Selection<T1, T2, T3>(selection.DbConnection,
                                                              selection.Relations[0],
                                                              selection.Relations[1],
                                                              selection.Relations[2]);
            CopySelection(selection, res);
            res.JoinOnSql = ConcatSqlConditions(selection.JoinOnSql,
                                                PredicateToSqlCondition.Convert(predicate,
                                                                                selection.DynamicParameters,
                                                                                selection.Relations[0],
                                                                                selection.Relations[1],
                                                                                selection.Relations[2]));
            return res;
        }
    }

    internal class Selection<T1, T2, T3, T4> : Selection, ISelection<T1, T2, T3, T4>
    {
        public Selection(IDbConnection dbConnection,
                         string relation1,
                         string relation2,
                         string relation3,
                         string relation4) : base(dbConnection)
        {
            Relations = new[] { relation1, relation2, relation3, relation4 };
        }

        public static Selection<T1, T2, T3, T4> AddWhereCondition(ISelection<T1, T2, T3, T4> selection, Expression<Func<T1, T2, T3, T4, bool>> predicate)
        {
            Selection<T1, T2, T3, T4> res = new Selection<T1, T2, T3, T4>(selection.DbConnection,
                                                                          selection.Relations[0],
                                                                          selection.Relations[1],
                                                                          selection.Relations[2],
                                                                          selection.Relations[3]);
            CopySelection(selection, res);
            res.WhereSql = ConcatSqlConditions(selection.WhereSql,
                                               PredicateToSqlCondition.Convert(predicate,
                                                                               selection.DynamicParameters,
                                                                               selection.Relations[0],
                                                                               selection.Relations[1],
                                                                               selection.Relations[2],
                                                                               selection.Relations[4]));
            return res;
        }

        public static Selection<T1, T2, T3, T4> AddJoinOnCondition(ISelection<T1, T2, T3, T4> selection, Expression<Func<T1, T2, T3, T4, bool>> predicate)
        {
            Selection<T1, T2, T3, T4> res = new Selection<T1, T2, T3, T4>(selection.DbConnection,
                                                                          selection.Relations[0],
                                                                          selection.Relations[1],
                                                                          selection.Relations[2],
                                                                          selection.Relations[3]);
            CopySelection(selection, res);
            res.JoinOnSql = ConcatSqlConditions(selection.JoinOnSql,
                                               PredicateToSqlCondition.Convert(predicate,
                                                                               selection.DynamicParameters,
                                                                               selection.Relations[0],
                                                                               selection.Relations[1],
                                                                               selection.Relations[2],
                                                                               selection.Relations[3]));
            return res;
        }
    }

    internal class Selection<T1, T2, T3, T4, T5> : Selection, ISelection<T1, T2, T3, T4, T5>
    {
        public Selection(IDbConnection dbConnection,
                         string relation1,
                         string relation2,
                         string relation3,
                         string relation4,
                         string relation5) : base(dbConnection)
        {
            Relations = new[] { relation1, relation2, relation3, relation4, relation5 };
        }

        public static Selection<T1, T2, T3, T4, T5> AddWhereCondition(ISelection<T1, T2, T3, T4, T5> selection, Expression<Func<T1, T2, T3, T4, T5, bool>> predicate)
        {
            Selection<T1, T2, T3, T4, T5> res = new Selection<T1, T2, T3, T4, T5>(selection.DbConnection,
                                                                                  selection.Relations[0],
                                                                                  selection.Relations[1],
                                                                                  selection.Relations[2],
                                                                                  selection.Relations[3],
                                                                                  selection.Relations[4]);
            CopySelection(selection, res);
            res.WhereSql = ConcatSqlConditions(selection.WhereSql,
                                               PredicateToSqlCondition.Convert(predicate,
                                                                               selection.DynamicParameters,
                                                                               selection.Relations[0],
                                                                               selection.Relations[1],
                                                                               selection.Relations[2],
                                                                               selection.Relations[3],
                                                                               selection.Relations[4]));
            return res;
        }

        public static Selection<T1, T2, T3, T4, T5> AddJoinOnCondition(ISelection<T1, T2, T3, T4, T5> selection, Expression<Func<T1, T2, T3, T4, T5, bool>> predicate)
        {
            Selection<T1, T2, T3, T4, T5> res = new Selection<T1, T2, T3, T4, T5>(selection.DbConnection,
                                                                                  selection.Relations[0],
                                                                                  selection.Relations[1],
                                                                                  selection.Relations[2],
                                                                                  selection.Relations[3],
                                                                                  selection.Relations[4]);
            CopySelection(selection, res);
            res.JoinOnSql = ConcatSqlConditions(selection.JoinOnSql,
                                               PredicateToSqlCondition.Convert(predicate,
                                                                               selection.DynamicParameters,
                                                                               selection.Relations[0],
                                                                               selection.Relations[1],
                                                                               selection.Relations[2],
                                                                               selection.Relations[3],
                                                                               selection.Relations[4]));
            return res;
        }
    }
}
