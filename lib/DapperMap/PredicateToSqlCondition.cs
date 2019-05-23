using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;

using Dapper;

namespace DapperMap
{
    internal class PredicateToSqlCondition
    {
        private int parametersCount = 0;
        private Dictionary<ParameterExpression, string> _relations = new Dictionary<ParameterExpression, string>();

        private static IReadOnlyDictionary<ExpressionType, string> SqlOpDict = new Dictionary<ExpressionType, string>()
        {
            {ExpressionType.Equal, " = "},
            {ExpressionType.NotEqual, " <> "},
            {ExpressionType.LessThan, " < "},
            {ExpressionType.GreaterThan, " > "},
            {ExpressionType.LessThanOrEqual, " <= "},
            {ExpressionType.GreaterThanOrEqual, " >= "},
            {ExpressionType.OrElse, " OR "},
            {ExpressionType.AndAlso, " AND "},
        };
        
        private PredicateToSqlCondition(LambdaExpression exp, DynamicParameters parameters, string[] relations)
        {
            Parameters = parameters;
            parametersCount = Parameters.ParameterNames.Count();
            int i = 0;
            foreach (var p in exp.Parameters)
                _relations.Add(p, relations[i++]);

            Sql = PredicateBodyParse(exp.Body);
        }

        public string Sql { get; private set; }
        public DynamicParameters Parameters { get; }

        private string PredicateBodyParse(Expression body)
        {
            if (!(body is BinaryExpression))
                throw new NotSupportedException($"Expression ({body}) must be binary");
            var binExp = (BinaryExpression)body;
            switch (binExp.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.LessThan:
                case ExpressionType.GreaterThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThanOrEqual:
                    {
                        var left = GetSqlFromMemberAcessOrConstant(binExp.Left);
                        var right = GetSqlFromMemberAcessOrConstant(binExp.Right);
                        if (left == null)
                            throw new ArgumentException("Expression can not contains compration with left side null object:" + binExp.ToString());
                        if (right == null)
                        {
                            if (binExp.NodeType == ExpressionType.Equal)
                                return left + " IS NULL";
                            if (binExp.NodeType == ExpressionType.NotEqual)
                                return left + " IS NOT NULL";
                        }

                        return left + SqlOpDict[body.NodeType] + right;
                    }
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    {
                        var left = PredicateBodyParse(binExp.Left);
                        var right = PredicateBodyParse(binExp.Right);

                        return left + SqlOpDict[body.NodeType] + right;
                    }
                default:
                    throw new NotSupportedException($"Expression ({binExp.ToString()}) has unsupported type: [{binExp.NodeType}]");
            }

        }

        string GetSqlFromMemberAcessOrConstant(Expression exp)
        {
            switch (exp)
            {
                case MemberExpression memberExp:
                    if (memberExp.Expression == null)
                    {
                        MemberInfo memberInfo = memberExp.Member;

                        switch (memberInfo) // static properties 
                        {
                            case FieldInfo fieldInfo:
                                return AddParameter(fieldInfo.GetValue(null));
                            case PropertyInfo propertyInfo:
                                return AddParameter(propertyInfo.GetValue(null));
                            default:
                                throw new NotSupportedException($"Member expression ({memberExp}) has unsupported member access: [{memberInfo.MemberType}]");
                        }
                    }
                    else
                    {
                        switch (memberExp.Expression)
                        {
                            case ParameterExpression paramExp:
                                {
                                    return _relations[paramExp] + "." + memberExp.Member.Name;
                                }
                            case ConstantExpression constExp: //may be closure 

                                MemberInfo memberInfo = memberExp.Member;

                                switch (memberInfo)
                                {
                                    case FieldInfo fieldInfo:
                                        return AddParameter(fieldInfo.GetValue(constExp.Value));
                                    case PropertyInfo propertyInfo:
                                        return AddParameter(propertyInfo.GetValue(constExp.Value));
                                    default:
                                        throw new NotSupportedException($"Member expression ({memberExp}) has unsupported member access: [{memberInfo.MemberType}]");
                                }
                            default:
                                if (memberExp.NodeType == ExpressionType.MemberAccess)
                                    throw new NotSupportedException($"Member expression ({memberExp}) has unsupported nested member access]");
                                else
                                    throw new NotSupportedException($"Member expression ({memberExp}) has unsupported type: [{memberExp.NodeType}]");
                        }
                    }
                case ConstantExpression constExp:
                    return AddParameter(constExp.Value);
                default:
                    throw new NotSupportedException($"Expression({exp}) has unsupported type: [{exp.NodeType}]");
            }
        }

        string AddParameter(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var param = "Param" + (parametersCount++);

            Parameters.Add(param, obj);

            return "@" + param;
        }

        public static string Convert<T>(Expression<Predicate<T>> expression, DynamicParameters parameters, string t1Relation)
        {
            var ptsc = new PredicateToSqlCondition(expression, parameters, new[] { t1Relation });
            return ptsc.Sql;
        }

        public static string Convert<T1, T2>(Expression<Func<T1, T2, bool>> expression, DynamicParameters parameters, string t1Relation, string t2Relation)
        {
            var ptsc = new PredicateToSqlCondition(expression, parameters, new[] { t1Relation, t2Relation });
            return ptsc.Sql;
        }

        public static string Convert<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> expression, DynamicParameters parameters, string t1Relation, string t2Relation, string t3Relation)
        {
            var ptsc = new PredicateToSqlCondition(expression, parameters, new[] { t1Relation, t2Relation, t3Relation });
            return ptsc.Sql;
        }
        
        public static string Convert<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> expression, DynamicParameters parameters, string t1Relation, string t2Relation, string t3Relation, string t4Relation)
        {
            var ptsc = new PredicateToSqlCondition(expression, parameters, new[] { t1Relation, t2Relation, t3Relation, t4Relation });
            return ptsc.Sql;
        }

        public static string Convert<T1, T2, T3, T4, T5>(Expression<Func<T1, T2, T3, T4, T5, bool>> expression, DynamicParameters parameters, string t1Relation, string t2Relation, string t3Relation, string t4Relation, string t5Relation)
        {
            var ptsc = new PredicateToSqlCondition(expression, parameters, new[] { t1Relation, t2Relation, t3Relation, t4Relation, t5Relation});
            return ptsc.Sql;
        }
    }
}
