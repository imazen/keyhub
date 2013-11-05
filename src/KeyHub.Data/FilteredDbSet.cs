using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;


namespace KeyHub.Data
{
    class FilteredDbSet<TEntity> : IDbSet<TEntity>, IOrderedQueryable<TEntity>, IOrderedQueryable, IQueryable<TEntity>, IQueryable, IEnumerable<TEntity>, IEnumerable, IListSource
        where TEntity : class
    {
        private readonly DbSet<TEntity> Set;
        private readonly IQueryable<TEntity> FilteredSet;
        private readonly Action<TEntity> InitializeEntity;

        public FilteredDbSet(DbContext context)
            : this(context.Set<TEntity>(), i => true, null)
        {
        }

        public FilteredDbSet(DbContext context, Expression<Func<TEntity, bool>> filter)
            : this(context.Set<TEntity>(), filter, null)
        {
        }

        public FilteredDbSet(DbContext context, Expression<Func<TEntity, bool>> filter, Action<TEntity> initializeEntity)
            : this(context.Set<TEntity>(), filter, initializeEntity)
        {
        }

        private FilteredDbSet(DbSet<TEntity> set, Expression<Func<TEntity, bool>> filter, Action<TEntity> initializeEntity)
        {
            Set = set;
            FilteredSet = set.Where(filter);
            MatchesFilter = filter.Compile();
            InitializeEntity = initializeEntity;
        }

        public Func<TEntity, bool> MatchesFilter { get; private set; }

        public void ThrowIfEntityDoesNotMatchFilter(TEntity entity)
        {
            //if (!MatchesFilter(entity))
            //    throw new ArgumentOutOfRangeException();
        }

        public TEntity Add(TEntity entity)
        {
            DoInitializeEntity(entity);
            ThrowIfEntityDoesNotMatchFilter(entity);
            return Set.Add(entity);
        }

        public TEntity Attach(TEntity entity)
        {
            ThrowIfEntityDoesNotMatchFilter(entity);
            return Set.Attach(entity);
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, TEntity
        {
            var entity = Set.Create<TDerivedEntity>();
            DoInitializeEntity(entity);
            return (TDerivedEntity)entity;
        }

        public TEntity Create()
        {
            var entity = Set.Create();
            DoInitializeEntity(entity);
            return entity;
        }

        public TEntity Find(params object[] keyValues)
        {
            var entity = Set.Find(keyValues);
            if (entity == null)
                return null;

            // If the user queried an item outside the filter, then we throw an error.
            // If IDbSet had a Detach method we would use it...sadly, we have to be ok with the item being in the Set.
            ThrowIfEntityDoesNotMatchFilter(entity);
            return entity;
        }

        public TEntity Remove(TEntity entity)
        {
            ThrowIfEntityDoesNotMatchFilter(entity);
            return Set.Remove(entity);
        }

        /// <summary>
        /// Returns the items in the local cache
        /// </summary>
        /// <remarks>
        /// It is possible to add/remove entities via this property that do NOT match the filter.
        /// Use the <see cref="ThrowIfEntityDoesNotMatchFilter"/> method before adding/removing an item from this collection.
        /// </remarks>
        public ObservableCollection<TEntity> Local { get { return Set.Local; } }

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator() { return FilteredSet.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return FilteredSet.GetEnumerator(); }

        Type IQueryable.ElementType { get { return typeof(TEntity); } }

        Expression IQueryable.Expression { get { return FilteredSet.Expression; } }

        IQueryProvider IQueryable.Provider { get { return FilteredSet.Provider; } }

        bool IListSource.ContainsListCollection { get { return false; } }

        IList IListSource.GetList() { throw new InvalidOperationException(); }

        void DoInitializeEntity(TEntity entity)
        {
            if (InitializeEntity != null)
                InitializeEntity(entity);
        }
    }
}
