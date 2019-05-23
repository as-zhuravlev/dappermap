using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Common;

using Dapper;

namespace DapperMap
{
    public abstract class BaseRepository
    {
        private bool _isConfugured = false;
        private Dictionary<Type, IMapping> _mappings;

        public BaseRepository(DbProviderFactory dbProviderFactory, string connectionString)
        {
            _mappings = new Dictionary<Type, IMapping>();
            DbProviderFactory = dbProviderFactory;
            ConnectionString = connectionString;
        }

        protected string ConnectionString { get; }
        protected DbProviderFactory DbProviderFactory { get; }

        protected int Add<T>(T entity) where T : new()
        {
            using (var conn = DbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = ConnectionString;
                return conn.QuerySingle<int>(GetMapping<T>().CreateSql, entity);
            }
        }

        protected void Delete<T>(int id) where T : new()
        {
            using (var conn = DbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = ConnectionString;
                conn.Execute(GetMapping<T>().DeleteSql, new { Id = id });
            }
        }

        protected IReadOnlyCollection<T> List<T>(
            Expression<Predicate<T>> wherePredicate = null) where T : new()
        {
            using (var conn = DbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = ConnectionString;
                var selection = conn.SelectFrom<T>(GetMapping<T>().SelectFromRelation);
                if (wherePredicate != null)
                    selection = selection.Where(wherePredicate);

                return selection.Map().ToList();
            }
        }

        protected IEnumerable<TResult> InnerJoin<T1, T2, TResult>
            (Func<T1, T2, TResult> map,
             Expression<Func<T1, T2, bool>> joinOnPredicate = null,
             Expression<Func<T1, T2, bool>> wherePredicate = null)
             where T1 : new()
             where T2 : new()
        {
            var t1 = GetMapping<T1>().SelectFromRelation;
            var t2 = GetMapping<T2>().SelectFromRelation;

            using (var conn = DbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = ConnectionString;
                var selection = conn.SelectFrom<T1, T2>(t1, t2);
                if (joinOnPredicate != null)
                    selection = selection.JoinOn(joinOnPredicate);
                if (wherePredicate != null)
                    selection = selection.Where(wherePredicate);

                return selection.Map(map).ToList();
            }
        }

        protected IEnumerable<TResult> InnerJoin<T1, T2, T3, TResult>
           (Func<T1, T2, T3, TResult> map,
            Expression<Func<T1, T2, T3, bool>> joinOnPredicate = null,
            Expression<Func<T1, T2, T3, bool>> wherePredicate = null)
            where T1 : new()
            where T2 : new()
            where T3 : new()
        {
            var t1 = GetMapping<T1>().SelectFromRelation;
            var t2 = GetMapping<T2>().SelectFromRelation;
            var t3 = GetMapping<T3>().SelectFromRelation;
            
            using (var conn = DbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = ConnectionString;
                var selection = conn.SelectFrom<T1, T2, T3>(t1, t2, t3);
                if (joinOnPredicate != null)
                    selection = selection.JoinOn(joinOnPredicate);
                if (wherePredicate != null)
                    selection = selection.Where(wherePredicate);

                return selection.Map(map).ToList();
            }
        }

        protected IEnumerable<TResult> InnerJoin<T1, T2, T3, T4, TResult>
           (Func<T1, T2, T3, T4, TResult> map,
            Expression<Func<T1, T2, T3, T4, bool>> joinOnPredicate = null,
            Expression<Func<T1, T2, T3, T4, bool>> wherePredicate = null)
            where T1 : new()
            where T2 : new()
            where T3 : new()
            where T4 : new()
        {
            var t1 = GetMapping<T1>().SelectFromRelation;
            var t2 = GetMapping<T2>().SelectFromRelation;
            var t3 = GetMapping<T3>().SelectFromRelation;
            var t4 = GetMapping<T4>().SelectFromRelation;
            
            using (var conn = DbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = ConnectionString;
                var selection = conn.SelectFrom<T1, T2, T3, T4>(t1, t2, t3, t4);
                if (joinOnPredicate != null)
                    selection = selection.JoinOn(joinOnPredicate);
                if (wherePredicate != null)
                    selection = selection.Where(wherePredicate);

                return selection.Map(map).ToList();
            }
        }

        protected IEnumerable<TResult> InnerJoin<T1, T2, T3, T4, T5, TResult>
        (Func<T1, T2, T3, T4, T5, TResult> map,
         Expression<Func<T1, T2, T3, T4, T5, bool>> joinOnPredicate = null,
         Expression<Func<T1, T2, T3, T4, T5, bool>> wherePredicate = null)
         where T1 : new()
         where T2 : new()
         where T3 : new()
         where T4 : new()
         where T5 : new()
        {
            var t1 = GetMapping<T1>().SelectFromRelation;
            var t2 = GetMapping<T2>().SelectFromRelation;
            var t3 = GetMapping<T3>().SelectFromRelation;
            var t4 = GetMapping<T4>().SelectFromRelation;
            var t5 = GetMapping<T5>().SelectFromRelation;
            
            using (var conn = DbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = ConnectionString;
                var selection = conn.SelectFrom<T1, T2, T3, T4, T5>(t1, t2, t3, t4, t5);
                if (joinOnPredicate != null)
                    selection = selection.JoinOn(joinOnPredicate);
                if (wherePredicate != null)
                    selection = selection.Where(wherePredicate);

                return selection.Map(map).ToList();
            }
        }

        protected void Update<T>(T entity) where T : new()
        {
            using (var conn = DbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = ConnectionString;
                conn.Execute(GetMapping<T>().UpdateSql, entity);
            }
        }

        protected void Update<T>(IReadOnlyCollection<T> entities) where T : new()
        {
            using (var conn = DbProviderFactory.CreateConnection())
            {
                conn.ConnectionString = ConnectionString;
                conn.Execute(GetMapping<T>().UpdateSql, entities);
            }
        }

        protected IMapping GetMapping<T>() where T : new()
        {
            if (!_isConfugured)
            {
                ConfigureMapping();
                _isConfugured = true;
            }

            var type = typeof(T);
            if (!_mappings.ContainsKey(type))
                throw new ArgumentException(type.Name, "Type is not mapped!");

            return _mappings[type];
        }

        protected void AddMapping<T>(IMapping mapping) where T : new()
        {
            _mappings.Add(typeof(T), mapping);
        }

        protected abstract void ConfigureMapping();
    }  
}
