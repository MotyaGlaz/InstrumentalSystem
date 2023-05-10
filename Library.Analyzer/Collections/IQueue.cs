using System.Collections;
using System.Collections.Generic;

namespace Library.Analyzer.Collections
{
    public interface IQueue<T> : IEnumerable<T>, ICollection, IEnumerable
    {
        void Enqueue(T item);
        T Dequeue();
        T Peek();
    }
}
