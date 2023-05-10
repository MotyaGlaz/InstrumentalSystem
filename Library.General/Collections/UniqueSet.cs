using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Library.General.Collections
{
    public class UniqueSet<T> :        
        ISet<T>, IReadOnlySet<T>
    {
        private readonly HashSet<T> _innerSet;

        public int Count { get { return _innerSet.Count; } }

        public bool IsReadOnly { get { return false; } }

        public UniqueSet()
        {
            _innerSet = new HashSet<T>();
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public bool Add(T item)
        {
            if (Contains(item))
                return false;
            return _innerSet.Add(item);
        }

        public bool Remove(T item)
        {
            T element = default(T);
            foreach (var elem in _innerSet)
            {
                if (elem.Equals(item))
                    element = elem;
            }
            return _innerSet.Remove(element);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                if (!Contains(item))
                    _innerSet.Add(item);
            }
        }

        public bool Contains(T item)
        {
            foreach (var element in _innerSet)
            {
                if (element.Equals(item))
                    return true;
            }
            return false;
        }

        public void Clear() => _innerSet.Clear();

        public void CopyTo(T[] array, int arrayIndex) => _innerSet.CopyTo(array, arrayIndex);

        public void ExceptWith(IEnumerable<T> other) => _innerSet.ExceptWith(other);

        public IEnumerator<T> GetEnumerator() => _innerSet.GetEnumerator();

        public void IntersectWith(IEnumerable<T> other) => _innerSet.IntersectWith(other);

        public bool IsProperSubsetOf(IEnumerable<T> other) => _innerSet.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => _innerSet.IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<T> other) => _innerSet.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) => _innerSet.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<T> other) => _innerSet.Overlaps(other);

        public bool SetEquals(IEnumerable<T> other) => _innerSet.SetEquals(other);

        public void SymmetricExceptWith(IEnumerable<T> other) => _innerSet.SymmetricExceptWith(other);

        IEnumerator IEnumerable.GetEnumerator() => _innerSet.GetEnumerator();

    }
}
