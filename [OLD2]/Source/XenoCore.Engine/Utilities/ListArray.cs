using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XenoCore.Engine
{
    public class ListArray<T> : IEnumerable<T> where T : class
    {
        private int count = 0;
        private T[] array;
        public int Size { get; private set; }
        public int Count
        {
            get
            {
                return count;
            }
        }

        public T this[int i]
        {
            get
            {
                Debug.AssertDebug(i < Count, "Index out of range!");
                return array[i];
            }

        }
        public ListArray(int size)
        {
            this.Size = size;
            array = new T[size];
            for (int i = 0; i < Size; ++i)
                array[i] = Activator.CreateInstance<T>();
        }


        public void Get(int i, out T value)
        {
            Debug.AssertDebug(i < Count, "Index out of range!");
            value = array[i];
        }
 
        public T New()
        {
            Debug.AssertDebug(count < Size, "Array is full!");
            return array[count++];
        }
        public T NewInterlocked()
        {
            return array[Interlocked.Increment(ref count) -1];
        }

        public void Remove(T obj)
        {
            for (int i = 0; i < count; ++i)
            {
                if (array[i] == obj)
                {
                    RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveAt(int index)
        {
            Debug.AssertDebug(count > 0, "Array is empty!");

            --count;
            if (count > 0)
            {
                var tmp = array[index];
                array[index] = array[count];
                array[count] = tmp;
            }
        }
        public void Clear()
        {
            count = 0;
        }
        public void ClearInterlocked()
        {
            Interlocked.Exchange(ref count, 0);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return array.Take(count).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return array.Take(count).GetEnumerator();
        }
    }
}
