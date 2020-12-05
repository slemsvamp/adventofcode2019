using System;

namespace day18
{
    public class Heap<T> where T : IHeapItem<T>
    {
        private readonly T[] items;
        private int currentItemCount;

        public Heap(int pMaxHeapSize)
        {
            if (pMaxHeapSize <= 0)
            {
                throw new Exception("MaxHeapSize must be larger than zero.");
            }

            items = new T[pMaxHeapSize];
        }

        public void Add(T pItem)
        {
            pItem.HeapIndex = currentItemCount;
            items[currentItemCount] = pItem;
            SortUp(pItem);
            currentItemCount++;
        }

        public T RemoveFirst()
        {
            T firstItem = items[0];
            currentItemCount--;
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return firstItem;
        }

        public bool Contains(T pItem)
        {
            return Equals(items[pItem.HeapIndex], pItem);
        }

        public void UpdateItem(T pItem)
        {
            SortUp(pItem);
        }

        public int Count
        {
            get
            {
                return currentItemCount;
            }
        }

        private void SortDown(T pItem)
        {
            while (true)
            {
                int childIndexLeft = pItem.HeapIndex * 2 + 1;
                int childIndexRight = pItem.HeapIndex * 2 + 2;
                int swapIndex = 0;

                if (childIndexLeft < currentItemCount)
                {
                    swapIndex = childIndexLeft;

                    if (childIndexRight < currentItemCount)
                    {
                        if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (pItem.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(pItem, items[swapIndex]);
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

        private void SortUp(T pItem)
        {
            int parentIndex = (pItem.HeapIndex - 1) / 2;
            while (true)
            {
                T parentItem = items[parentIndex];
                if (pItem.CompareTo(parentItem) > 0)
                {
                    Swap(pItem, parentItem);
                }
                else
                {
                    break;
                }

                parentIndex = (pItem.HeapIndex - 1) / 2;
            }
        }

        private void Swap(T pItemA, T pItemB)
        {
            items[pItemA.HeapIndex] = pItemB;
            items[pItemB.HeapIndex] = pItemA;
            int itemAIndex = pItemA.HeapIndex;
            pItemA.HeapIndex = pItemB.HeapIndex;
            pItemB.HeapIndex = itemAIndex;
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }
}
