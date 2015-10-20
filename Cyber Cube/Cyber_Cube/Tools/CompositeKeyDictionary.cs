using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Tools
{
    [Serializable]
    public class CompositeKeyDictionary<K1, K2, V> : Dictionary<Tuple<K1, K2>, V>
    {

        private Dictionary<K1, HashSet<K2>> mKeyset = new Dictionary<K1, HashSet<K2>>();

        public bool ContainsKey( K1 key1, K2 key2 )
        {
            return base.ContainsKey( Tuple.Create( key1, key2 ) );
        }

        public bool ContainsKey( K1 key1 )
        {
            return mKeyset.ContainsKey( key1 );
        }

        public new bool Remove( Tuple<K1, K2> compKey )
        {
            if ( mKeyset.ContainsKey( compKey.Item1 ) )
                mKeyset[ compKey.Item1 ].Remove( compKey.Item2 );

            return base.Remove( compKey );
        }

        public bool Remove( K1 key1, K2 key2 )
        {
            return Remove( Tuple.Create( key1, key2 ) );
        }

        public int RemoveAll( K1 key1 )
        {
            int removed = 0;

            if ( mKeyset.ContainsKey( key1 ) )
            {
                foreach ( K2 key2 in mKeyset[ key1 ] )
                    removed += base.Remove( Tuple.Create( key1, key2 ) ) ? 1 : 0;

                mKeyset.Remove( key1 );
            }

            return removed;
        }

        public new V this[ Tuple<K1, K2> compKey ]
        {
            get {
                return base[ compKey ];
            }
            set {
                if ( !mKeyset.ContainsKey( compKey.Item1 ) )
                    mKeyset[ compKey.Item1 ] = new HashSet<K2>();

                mKeyset[ compKey.Item1 ].Add( compKey.Item2 );

                base[ compKey ] = value;
            }
        }

        public V this[ K1 key1, K2 key2 ]
        {
            get {
                return this[ Tuple.Create( key1, key2 ) ];
            }
            set { 
                this[ Tuple.Create( key1, key2 ) ] = value;
            }
        }

        public IEnumerable<V> this[ K1 key1 ]
        {
            get {
                foreach ( K2 key2 in mKeyset[ key1 ] )
                    yield return this[ key1, key2 ];
            }
        }

    }
}
