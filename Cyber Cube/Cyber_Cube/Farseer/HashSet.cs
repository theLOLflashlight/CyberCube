using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarseerPhysics
{
    public class HashSet<T> : ICollection<T>
    {
        private Dictionary<T, Int16> dict;
        // Dictionary<T, bool>

        public HashSet()
        {
            dict = new Dictionary<T, short>();
        }

        public HashSet( HashSet<T> from )
        {
            dict = new Dictionary<T, short>();
            foreach ( T n in from )
                dict.Add( n, 0 );
        }

        public void Add( T item )
        {
            // The key of the dictionary is used but not the value.
            dict.Add( item, 0 );
        }

        public void Clear()
        {
            dict.Clear();
        }

        public bool Contains( T item )
        {
            return dict.ContainsKey( item );
        }

        public void CopyTo(
            T[] array,
            int arrayIndex )
        {
            throw new NotImplementedException();
        }

        public bool Remove( T item )
        {
            return dict.Remove( item );
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable) dict.Keys).GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) dict.Keys).GetEnumerator();
        }

        public int Count
        {
            get { return dict.Keys.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}
