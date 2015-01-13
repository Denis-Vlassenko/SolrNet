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
        }

        public IEnumerable<ISolrClusterReplica> Active {
            get { return base.Values.Where(replica => replica.IsActive); }
        }

        public IEnumerable<ISolrClusterReplica> Leaders {
            get { return base.Values.Where(replica => replica.IsLeader); }
        }

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
