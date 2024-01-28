using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemVerifikation
{
    public class KeyValuePairComparer : IEqualityComparer<KeyValuePair<string, bool>>
    {
        public static readonly KeyValuePairComparer Instance = new KeyValuePairComparer();

        public bool Equals(KeyValuePair<string, bool> x, KeyValuePair<string, bool> y)
        {
            return x.Key == y.Key && x.Value == y.Value;
        }

        public int GetHashCode(KeyValuePair<string, bool> obj)
        {
            return obj.Key.GetHashCode() ^ obj.Value.GetHashCode();
        }
    }
}
