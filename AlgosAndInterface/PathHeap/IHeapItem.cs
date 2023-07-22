using System;

namespace FeralPug.PathFinding
{
    public interface IHeapItem<T> : IComparable<T>
    {
        public int HeapIndex { get; set; }
    }
}
