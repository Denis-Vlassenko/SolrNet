using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud.Playground
{
    public class SolrClusterRouter : ISolrClusterRouter {
        public SolrClusterRouter(JObject json) {
            Name = (string) json["name"];
        }
        public string Name { get; private set; }
    }
}
