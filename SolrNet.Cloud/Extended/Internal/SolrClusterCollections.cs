using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SolrNet.Cloud {
    internal class SolrClusterCollections : ISolrClusterCollections
    {
        public SolrClusterCollections(IEnumerable<ISolrClusterCollection> collections) {
            var memoized = collections.ToList();
            dictionary = new Dictionary<string, ISolrClusterCollection>(memoized.Count, StringComparer.OrdinalIgnoreCase);
            list = new List<ISolrClusterCollection>(memoized.Count);
            foreach (var collection in memoized)
            {
                dictionary.Add(collection.Name, collection);
                list.Add(collection);
            }
        }

        private readonly Dictionary<string, ISolrClusterCollection> dictionary;

        private readonly List<ISolrClusterCollection> list;

        public ISolrClusterCollection this[int index] {
            get {
                return index >= 0 && index < list.Count
                    ? list[index]
                    : null;
            }
        }

        ISolrClusterCollection ISolrClusterCollections.this[string name] {
            get {
                if (name == null)
                    return list[0];
                ISolrClusterCollection core;
                dictionary.TryGetValue(name, out core);
                return core;
            }
        }

        public int Count {
            get { return list.Count; }
        }

        public IEnumerator<ISolrClusterCollection> GetEnumerator() {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}