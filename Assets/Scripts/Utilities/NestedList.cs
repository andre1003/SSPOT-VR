using System;
using System.Collections;
using System.Collections.Generic;

namespace SSPot.Utilities
{
    /// <summary>
    /// Represents a list that can be used with NaughtyAttributes.
    /// This class is designed to work around Unity's default behavior of
    /// applying attributes such as <see cref="NaughtyAttributes.HideIfAttribute"/> to each individual item in a list.
    /// By using this class, you can apply NaughtyAttributes to
    /// the list as a whole, rather than to each element separately.
    /// </summary>
    /// <remarks>Use with <see cref="SSPot.Utilities.Attributes.InlineAttribute"/> to draw in a single line.</remarks>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    [Serializable]
    public class NestedList<T> : IList<T>
    {
        public List<T> List = new();
        
        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) List).GetEnumerator();
        }

        public void Add(T item)
        {
            List.Add(item);
        }

        public void Clear()
        {
            List.Clear();
        }

        public bool Contains(T item)
        {
            return List.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return List.Remove(item);
        }

        public int Count => List.Count;

        public bool IsReadOnly => false;

        public int IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            List.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
        }

        public T this[int index]
        {
            get => List[index];
            set => List[index] = value;
        }
    }
}