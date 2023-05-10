using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Library.General.Collections
{
    public class UniqueObservableCollection<T> : ObservableCollection<T>
    {
        private ObservableCollection<T> _collection;

        public new int Count { get { return _collection.Count; } }

        public UniqueObservableCollection()
        {
            _collection = new ObservableCollection<T>();
        }
        public UniqueObservableCollection(IEnumerable<T> collection)
        {
            _collection = new ObservableCollection<T>(collection);
        }
        public UniqueObservableCollection(List<T> list)
        {
            _collection = new ObservableCollection<T>(list);
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        public new void Add(T item)
        {
            if (!Contains(item))
                _collection.Add(item);
        }

        public new bool Remove(T item)
        {
            var toRemove = default(T);
            foreach (var element in _collection)
                if (element.GetHashCode().Equals(item.GetHashCode()))
                    toRemove = element;
            return _collection.Remove(toRemove);
        }

        public new void RemoveAt(int index)
        {
            _collection.RemoveAt(index);
        }

        public new T this[int index]
        {
            get => _collection[index];
            set => _collection[index] = value;
        }

        public new bool Contains(T item)
        {
            if (item is null)
                return true;
            foreach (var element in _collection)
                if (element.GetHashCode().Equals(item.GetHashCode()))
                    return true;

            return false;
        }
       
    }
}
