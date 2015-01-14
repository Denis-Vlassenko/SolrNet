using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud {
    public class SolrClusterRouter : ISolrClusterRouter {
        public SolrClusterRouter(ISolrClusterCore core, JObject json) {
            Core = core;
            Name = (string) json["name"];
        }

        public ISolrClusterCore Core { get; private set; }

        public string Name { get; private set; }
    }
}
