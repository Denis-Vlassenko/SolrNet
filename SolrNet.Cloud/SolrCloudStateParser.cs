using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud {
    public static class SolrCloudStateParser {
        private static ISolrCloudNode BuildNode(string collection, int? rangeEnd, int? rangeStart, JProperty json) {
            var baseUrl = (string) json.Value["base_url"];
            var leader = json.Value["leader"];
            var state = (string) json.Value["state"];
            return new SolrCloudNode(
                collection,
                "active".Equals(state, StringComparison.OrdinalIgnoreCase),
                leader != null && (bool) leader,
                rangeEnd,
                rangeStart,
                baseUrl + "/" + collection);
        }

        public static IEnumerable<ISolrCloudNode> ParseJsonToNodes(string json) {
            foreach (var collection in JObject.Parse(json).Properties()) {
                foreach (var shard in ((JObject) collection.Value["shards"]).Properties()) {
                    var range = (string) shard.Value["range"];
                    int? rangeEnd = null;
                    int? rangeStart = null;
                    if (!string.IsNullOrEmpty(range)) {
                        var parts = range.Split('-');
                        rangeStart = int.Parse(parts[0], NumberStyles.HexNumber);
                        rangeEnd = int.Parse(parts[1], NumberStyles.HexNumber);
                    }
                    foreach (var replica in ((JObject) shard.Value["replicas"]).Properties())
                        yield return BuildNode(collection.Name, rangeEnd, rangeStart, replica);
                }
            }
        }
    }
}