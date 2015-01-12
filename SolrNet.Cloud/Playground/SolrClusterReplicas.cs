using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud.Playground
{
    public class SolrClusterReplicas : Dictionary<string, ISolrClusterReplica>, ISolrClusterReplicas
    {
        public SolrClusterReplicas(JObject json) {
            foreach (var property in json.Properties()) {
                var replica = new SolrClusterReplica(property);
                base.Add(replica.Name, replica);
            }
            Active = base.Values.Where(replica => replica.IsActive).ToArray();
            Leaders = base.Values.Where(replica => replica.IsLeader).ToArray();
        }

        public IEnumerable<ISolrClusterReplica> Active { get; private set; }
        public IEnumerable<ISolrClusterReplica> Leaders { get; private set; }

        IEnumerator<ISolrClusterReplica> IEnumerable<ISolrClusterReplica>.GetEnumerator()
        {
            return base.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
