using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace LMS.Core.Interfaces
{
    public interface IRepository
    {
        int Add<T>(T entity) where T  : IEntity, new();

        void Delete<T>(int id) where T : IEntity, new();

        void Update<T>(T entity) where T : IEntity, new();

        IReadOnlyCollection<T> List<T>(
            Expression<Predicate<T>> tWherePredicate)
            where T : IEntity, new();

        IEnumerable<TResult> InnerJoin<T1, T2, TResult>(
            Func<T1, T2, TResult> map,
            Expression<Func<T1, T2, bool>> joinOnPredicate,
            Expression<Func<T1, T2, bool>> wherePredicate
            )
            where T1 : IEntity, new()
            where T2 : IEntity, new();


        IEnumerable<TResult> InnerJoin<T1, T2, T3, TResult>(
            Func<T1, T2, T3, TResult> map,
            Expression<Func<T1, T2, T3, bool>> joinOnPredicate,
            Expression<Func<T1, T2, T3, bool>> wherePredicate
            )
            where T1 : IEntity, new()
            where T2 : IEntity, new()
            where T3 : IEntity, new();

        IEnumerable<TResult> InnerJoin<T1, T2, T3, T4, TResult>(
            Func<T1, T2, T3, T4, TResult> map,
            Expression<Func<T1, T2, T3, T4, bool>> joinOnPredicate,
            Expression<Func<T1, T2, T3, T4, bool>> wherePredicate
            )
            where T1 : IEntity, new()
            where T2 : IEntity, new()
            where T3 : IEntity, new()
            where T4 : IEntity, new();

        IEnumerable<TResult> InnerJoin<T1, T2, T3, T4, T5, TResult>(
            Func<T1, T2, T3, T4, T5, TResult> map,
            Expression<Func<T1, T2, T3, T4, T5, bool>> joinOnPredicate,
            Expression<Func<T1, T2, T3, T4, T5, bool>> wherePredicate
            )
            where T1 : IEntity, new()
            where T2 : IEntity, new()
            where T3 : IEntity, new()
            where T4 : IEntity, new()
            where T5 : IEntity, new();
    }
}
