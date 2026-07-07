using System;
using System.Collections.Generic;

namespace CatAnnaDev.Utils
{
    public sealed class PriorityQueue<TElement>
    {
        private struct Node
        {
            public TElement Element;
            public float Priority;
        }

        private Node[] heap;
        private int count;
        private readonly bool minHeap;

        public PriorityQueue(bool minHeap = true)
            : this(minHeap, 16)
        {
        }

        public PriorityQueue(bool minHeap, int capacity)
        {
            this.minHeap = minHeap;
            heap = new Node[Math.Max(1, capacity)];
            count = 0;
        }

        public int Count
        {
            get { return count; }
        }

        public bool IsEmpty
        {
            get { return count == 0; }
        }

        public void Clear()
        {
            Array.Clear(heap, 0, count);
            count = 0;
        }

        public void Enqueue(TElement element, float priority)
        {
            if (count == heap.Length)
            {
                Array.Resize(ref heap, heap.Length * 2);
            }
            Node node = new Node { Element = element, Priority = priority };
            heap[count] = node;
            SiftUp(count);
            count++;
        }

        public TElement Peek()
        {
            if (count == 0)
            {
                throw new InvalidOperationException("PriorityQueue is empty.");
            }
            return heap[0].Element;
        }

        public bool TryPeek(out TElement element, out float priority)
        {
            if (count == 0)
            {
                element = default;
                priority = 0f;
                return false;
            }
            element = heap[0].Element;
            priority = heap[0].Priority;
            return true;
        }

        public TElement Dequeue()
        {
            if (count == 0)
            {
                throw new InvalidOperationException("PriorityQueue is empty.");
            }
            Node root = heap[0];
            count--;
            heap[0] = heap[count];
            heap[count] = default;
            if (count > 0)
            {
                SiftDown(0);
            }
            return root.Element;
        }

        public bool TryDequeue(out TElement element, out float priority)
        {
            if (count == 0)
            {
                element = default;
                priority = 0f;
                return false;
            }
            element = heap[0].Element;
            priority = heap[0].Priority;
            Dequeue();
            return true;
        }

        private bool Compare(float a, float b)
        {
            return minHeap ? a < b : a > b;
        }

        private void SiftUp(int index)
        {
            Node node = heap[index];
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (!Compare(node.Priority, heap[parent].Priority))
                {
                    break;
                }
                heap[index] = heap[parent];
                index = parent;
            }
            heap[index] = node;
        }

        private void SiftDown(int index)
        {
            Node node = heap[index];
            int half = count / 2;
            while (index < half)
            {
                int left = index * 2 + 1;
                int right = left + 1;
                int best = left;
                if (right < count && Compare(heap[right].Priority, heap[left].Priority))
                {
                    best = right;
                }
                if (!Compare(heap[best].Priority, node.Priority))
                {
                    break;
                }
                heap[index] = heap[best];
                index = best;
            }
            heap[index] = node;
        }
    }
}
