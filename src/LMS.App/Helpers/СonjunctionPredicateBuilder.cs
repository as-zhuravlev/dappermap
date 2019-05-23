using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LMS.App.Helpers
{
    public class СonjunctionPredicateBuilder
    {
        private readonly List<ParameterExpression> _parameters;
        private Expression _body { get; set; }

        public СonjunctionPredicateBuilder()
        {
            _parameters = new List<ParameterExpression>();
        }

        public LambdaExpression Lambda => Expression.Lambda(_body ?? Expression.Constant(true), _parameters);
        
        public bool IsEmpty => _body == null;
        
        public void AddArgWithPropertyEqComparation<TArg, TProperty>(Expression<Func<TArg, TProperty>> propertySelector, TProperty equalObj)
        {
            if (!(propertySelector.Body is MemberExpression body))
                throw new NotSupportedException($"PropertySelector has unsupported expression type [{propertySelector.Body.NodeType}]");

            if (body.Expression.NodeType != ExpressionType.Parameter)
                throw new NotSupportedException($"Nested member access is unsupported [{body}]");

            if (body.Member.MemberType != MemberTypes.Property)
                throw new NotSupportedException($"Unsupported member access type [{body.Member.MemberType}]");

            var comparation = Expression.Equal(propertySelector.Body, Expression.Constant(equalObj));
            _body = _body == null ? comparation : Expression.AndAlso(_body, comparation);
            _parameters.Add(propertySelector.Parameters.First());
        }

        public void AddPropertyEqComparation<TArg, TProperty>(Expression<Func<TArg, TProperty>> propertySelector, TProperty equalObj)
        {
            if (_parameters.Count == 0)
                _parameters.Add(propertySelector.Parameters.First());

            if (_parameters.Last().Type != typeof(TArg))
                throw new InvalidOperationException($"Invalid lambda parameter type [{nameof(TArg)}]. Type should be same with latest added argument type {_parameters.Last().Type.Name}");

            if (!(propertySelector.Body is MemberExpression body))
                throw new NotSupportedException($"PropertySelector has unsupported expression type [{propertySelector.Body.NodeType}]");

            if (body.Expression.NodeType != ExpressionType.Parameter)
                throw new NotSupportedException($"Nested member access is unsupported [{body}]");

            if (body.Member.MemberType != MemberTypes.Property)
                throw new NotSupportedException($"Unsupported member access type [{body.Member.MemberType}]");

            MemberExpression me = Expression.MakeMemberAccess(_parameters.Last(), body.Member);

            var comparation = Expression.Equal(me, Expression.Constant(equalObj));

            _body = _body == null ? comparation : Expression.AndAlso(_body, comparation);
        }

        public void AddArg<TArg>()
        {
            _parameters.Add(Expression.Parameter(typeof(TArg)));
        }
    }
}
