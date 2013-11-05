using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace KeyHub.Common.Collections
{
    /// <summary>
    /// Represents a adapting collection that adapts to certain constrictions
    /// </summary>
    public class AdaptingCollection<T> : AdaptingCollection<T, IDictionary<string, object>>
    {
        public AdaptingCollection(Func<IEnumerable<Lazy<T, IDictionary<string, object>>>,
                                       IEnumerable<Lazy<T, IDictionary<string, object>>>> adaptor)
            : base(adaptor)
        {
        }
    }

    /// <summary>
    /// Represents a adapting collection that adapts to certain constrictions
    /// </summary>
    public class AdaptingCollection<T, M> : ICollection<Lazy<T, M>>, INotifyCollectionChanged
    {
        private readonly List<Lazy<T, M>> allItems = new List<Lazy<T, M>>();
        private readonly Func<IEnumerable<Lazy<T, M>>, IEnumerable<Lazy<T, M>>> adaptor = null;
        private List<Lazy<T, M>> adaptedItems = null;

        /// <summary>
        /// Creates a new AdaptingCollection
        /// </summary>
        public AdaptingCollection()
            : this(null)
        {
        }

        /// <summary>
        /// Creates a new AdaptingCollection
        /// </summary>
        public AdaptingCollection(Func<IEnumerable<Lazy<T, M>>, IEnumerable<Lazy<T, M>>> adaptor)
        {
            this.adaptor = adaptor;
        }

        /// <summary>
        /// Event to notify when the collection has changed
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Re-applies the Adaptor and raises the CollectionChanged event
        /// </summary>
        public void ReapplyAdaptor()
        {
            if (this.adaptedItems != null)
            {
                this.adaptedItems = null;
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        protected virtual IEnumerable<Lazy<T, M>> Adapt(IEnumerable<Lazy<T, M>> collection)
        {
            if (this.adaptor != null)
            {
                return this.adaptor.Invoke(collection);
            }

            return collection;
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler collectionChanged = this.CollectionChanged;

            if (collectionChanged != null)
            {
                collectionChanged.Invoke(this, e);
            }
        }

        private List<Lazy<T, M>> AdaptedItems
        {
            get
            {
                if (this.adaptedItems == null)
                {
                    this.adaptedItems = Adapt(this.allItems).ToList();
                }

                return this.adaptedItems;
            }
        }

        #region ICollection Implementation

        /// <summary>
        /// Accessors work directly against adapted collection
        /// </summary>
        public bool Contains(Lazy<T, M> item)
        {
            return this.AdaptedItems.Contains(item);
        }

        /// <summary>
        /// Copies the items to a new array
        /// </summary>
        public void CopyTo(Lazy<T, M>[] array, int arrayIndex)
        {
            this.AdaptedItems.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns the number of items in the collection
        /// </summary>
        public int Count
        {
            get { return this.AdaptedItems.Count; }
        }

        /// <summary>
        /// Get wether the collection is readonly
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns the collection Enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Lazy<T, M>> GetEnumerator()
        {
            return this.AdaptedItems.GetEnumerator();
        }

        /// <summary>
        /// Returns the collections generic Enumerator
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Mutation methods work against complete collection and then force a reset of the adapted collection
        /// </summary>
        /// <param name="item"></param>
        public void Add(Lazy<T, M> item)
        {
            this.allItems.Add(item);
            ReapplyAdaptor();
        }

        /// <summary>
        /// Clears the collection
        /// </summary>
        public void Clear()
        {
            this.allItems.Clear();
            ReapplyAdaptor();
        }

        /// <summary>
        /// Removes the given item from the collection
        /// </summary>
        public bool Remove(Lazy<T, M> item)
        {
            bool removed = this.allItems.Remove(item);
            ReapplyAdaptor();
            return removed;
        }

        #endregion ICollection Implementation
    }
}