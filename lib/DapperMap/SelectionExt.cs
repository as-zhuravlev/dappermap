using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

using Dapper;

namespace DapperMap
{
    public static class SelectionFromExt
    {
        public static ISelection<T> SelectFrom<T>(this IDbConnection dbConnection, string relation)
        {
            return new Selection<T>(dbConnection, relation);
        }

        public static ISelection<T> Where<T>(this ISelection<T> selection, Expression<Predicate<T>> predicate)
        {
            return Selection<T>.AddWhereCondition(selection, predicate);
        }

        private static string BuildSqlQuery(ISelection selection)
        {
            string sql = selection.SelectSql;

            string condition = Selection.ConcatSqlConditions(selection.JoinOnSql, selection.WhereSql);
            
            if (!string.IsNullOrWhiteSpace(selection.WhereSql))
                sql += " WHERE " + condition;

            return sql;
        }

        public static IEnumerable<T> Map<T>(this ISelection<T> selection,
                                            IDbTransaction dbTransaction = null,
                                            bool buffered = true,
                                            int? commandTimeout = null,
                                            CommandType? commandType = null)
        {
            string sql = selection.SelectSql;

            if (!string.IsNullOrWhiteSpace(selection.WhereSql))
                sql = " WHERE " + selection.WhereSql;

            return selection.DbConnection.Query<T>(sql,
                                                   selection.DynamicParameters,
                                                   dbTransaction,
                                                   buffered,
                                                   commandTimeout, commandType);
        }

        public static ISelection<T1, T2> SelectFrom<T1, T2>(this IDbConnection dbConnection, string relation1, string relation2)
        {
            return new Selection<T1, T2>(dbConnection, relation1, relation2);
        }

        public static ISelection<T1, T2> JoinOn<T1, T2>(this ISelection<T1, T2> selection, Expression<Func<T1, T2, bool>> predicate)
        {
            return Selection<T1, T2>.AddJoinOnCondition(selection, predicate);
        }

        public static ISelection<T1, T2> Where<T1, T2>(this ISelection<T1, T2> selection, Expression<Func<T1, T2, bool>> predicate)
        {
            return Selection<T1, T2>.AddWhereCondition(selection, predicate);
        }

        public static IEnumerable<TResult> Map<T1, T2, TResult>(this ISelection<T1, T2> selection,
                                                               Func<T1, T2, TResult> map,
                                                               IDbTransaction dbTransaction = null,
                                                               bool buffered = true,
                                                               string splitOn = "Id",
                                                               int? commandTimeout = null,
                                                               CommandType? commandType = null)
        {
            return selection.DbConnection.Query(BuildSqlQuery(selection),
                                                map,
                                                selection.DynamicParameters,
                                                dbTransaction,
                                                buffered,
                                                splitOn,
                                                commandTimeout, commandType);
        }

        public static ISelection<T1, T2, T3> SelectFrom<T1, T2, T3>(this IDbConnection dbConnection, string relation1, string relation2, string relation3)
        {
            return new Selection<T1, T2, T3>(dbConnection, relation1, relation2, relation3);
        }

        public static ISelection<T1, T2, T3> JoinOn<T1, T2, T3>(this ISelection<T1, T2, T3> selection, Expression<Func<T1, T2, T3, bool>> predicate)
        {
            return Selection<T1, T2, T3>.AddJoinOnCondition(selection, predicate);
        }

        public static ISelection<T1, T2, T3> Where<T1, T2, T3>(this ISelection<T1, T2, T3> selection, Expression<Func<T1, T2, T3, bool>> predicate)
        {
            return Selection<T1, T2, T3>.AddWhereCondition(selection, predicate);
        }

        public static IEnumerable<TResult> Map<T1, T2, T3, TResult>(this ISelection<T1, T2, T3> selection,
                                                                    Func<T1, T2, T3, TResult> map,
                                                                    IDbTransaction dbTransaction = null,
                                                                    bool buffered = true,
                                                                    string splitOn = "Id",
                                                                    int? commandTimeout = null,
                                                                    CommandType? commandType = null)
        {
            return selection.DbConnection.Query(BuildSqlQuery(selection),
                                                map,
                                                selection.DynamicParameters,
                                                dbTransaction,
                                                buffered,
                                                splitOn,
                                                commandTimeout, commandType);
        }

        public static ISelection<T1, T2, T3, T4> SelectFrom<T1, T2, T3, T4>(this IDbConnection dbConnection, string relation1, string relation2, string relation3, string relation4)
        {
            return new Selection<T1, T2, T3, T4>(dbConnection, relation1, relation2, relation3, relation4);
        }

        public static ISelection<T1, T2, T3, T4> JoinOn<T1, T2, T3, T4>(this ISelection<T1, T2, T3, T4> selection, Expression<Func<T1, T2, T3, T4, bool>> predicate)
        {
            return Selection<T1, T2, T3, T4>.AddJoinOnCondition(selection, predicate);
        }

        public static ISelection<T1, T2, T3, T4> Where<T1, T2, T3, T4>(this ISelection<T1, T2, T3, T4> selection, Expression<Func<T1, T2, T3, T4, bool>> predicate)
        {
            return Selection<T1, T2, T3, T4>.AddWhereCondition(selection, predicate);
        }

        public static IEnumerable<TResult> Map<T1, T2, T3, T4, TResult>(this ISelection<T1, T2, T3, T4> selection,
                                                                        Func<T1, T2, T3, T4, TResult> map,
                                                                        IDbTransaction dbTransaction = null,
                                                                        bool buffered = true,
                                                                        string splitOn = "Id",
                                                                        int? commandTimeout = null,
                                                                        CommandType? commandType = null)
        {
            return selection.DbConnection.Query(BuildSqlQuery(selection),
                                                map,
                                                selection.DynamicParameters,
                                                dbTransaction,
                                                buffered,
                                                splitOn,
                                                commandTimeout, commandType);
        }

        public static ISelection<T1, T2, T3, T4, T5> SelectFrom<T1, T2, T3, T4, T5>(this IDbConnection dbConnection, string relation1, string relation2, string relation3, string relation4, string relation5)
        {
            return new Selection<T1, T2, T3, T4, T5>(dbConnection, relation1, relation2, relation3, relation4, relation5);
        }

        public static ISelection<T1, T2, T3, T4, T5> JoinOn<T1, T2, T3, T4, T5>(this ISelection<T1, T2, T3, T4, T5> selection, Expression<Func<T1, T2, T3, T4, T5, bool>> predicate)
        {
            return Selection<T1, T2, T3, T4, T5>.AddJoinOnCondition(selection, predicate);
        }

        public static ISelection<T1, T2, T3, T4, T5> Where<T1, T2, T3, T4, T5>(this ISelection<T1, T2, T3, T4, T5> selection, Expression<Func<T1, T2, T3, T4, T5, bool>> predicate)
        {
            return Selection<T1, T2, T3, T4, T5>.AddWhereCondition(selection, predicate);
        }

        public static IEnumerable<TResult> Map<T1, T2, T3, T4, T5, TResult>(this ISelection<T1, T2, T3, T4, T5> selection,
                                                                        Func<T1, T2, T3, T4, T5, TResult> map,
                                                                        IDbTransaction dbTransaction = null,
                                                                        bool buffered = true,
                                                                        string splitOn = "Id",
                                                                        int? commandTimeout = null,
                                                                        CommandType? commandType = null)
        {
            return selection.DbConnection.Query(BuildSqlQuery(selection),
                                                map,
                                                selection.DynamicParameters,
                                                dbTransaction,
                                                buffered,
                                                splitOn,
                                                commandTimeout, commandType);
        }
    }

}
