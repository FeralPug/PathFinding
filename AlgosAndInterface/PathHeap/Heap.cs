using System.Collections.Generic;

namespace FeralPug.PathFinding
{
    //https://github.com/SebLague/Pathfinding/blob/master/Episode%2004%20-%20heap/Assets/Scripts/Heap.cs
    public class Heap<T> where T : class, IHeapItem<T>
    {
        T[] items;
        HashSet<T> record;
        int currentItemCount;

        public int Count => currentItemCount;

        public Heap(int maxItemCount)
        {
            items = new T[maxItemCount];
            record = new HashSet<T>();
            currentItemCount = 0;
        }

        public void Add(T item)
        {
            item.HeapIndex = currentItemCount;
            record.Add(item);
            items[currentItemCount++] = item;
            SortUp(item);
        }

        public T RemoveFirst()
        {
            var removeItem = items[0];
            record.Remove(removeItem);
            currentItemCount--;

            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            
            return removeItem;
        }

        public void UpdateItem(T item)
        {
            //item should have been grabbed with GetRecord, otherwise will not work correctly
            if (ContainsItem(item))
            {
                SortUp(item);
            }
        }

        public T GetRecord(T item)
        {
            if(record.TryGetValue(item, out T recordItem))
            {
                return recordItem;
            }

            return null;
        }

        public bool ContainsItem(T item)
        {
            return record.Contains(item);
            //return Equals(items[item.HeapIndex], item);
        }

        void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                var parent = items[parentIndex];
                if(item.CompareTo(parent) < 0)
                {
                    Swap(item, parent);
                }
                else
                {
                    break;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        void SortDown(T item)
        {
            while (true)
            {
                int leftChildIndex = item.HeapIndex * 2 + 1;
                int rightChildIndex = item.HeapIndex * 2 + 2;

                if(leftChildIndex < Count)
                {
                    int swapIndex = leftChildIndex;

                    if (rightChildIndex < Count)
                    {
                        if (items[rightChildIndex].CompareTo(items[swapIndex]) < 0)
                        {
                            swapIndex = rightChildIndex;
                        }
                    }

                    if (item.CompareTo(items[swapIndex]) > 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        void Swap(T itemA, T itemB)
        {
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;
            int aIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = aIndex;
        }
    }
}
