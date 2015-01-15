using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud {
    public static partial class SolrClusterStateParser {
        private static ISolrClusterCollection BuildCollection(JProperty json) {
            var collection = new SolrClusterCollection {
                Name = json.Name,
                Router = BuildRouter(json.Value["router"] as JObject)
            };
            collection.Shards = BuildShards(collection, json.Value["shards"] as JObject);
            return collection;
        }

        private static ISolrClusterCollections BuildCollections(JObject json) {
            return new SolrClusterCollections(
                json.Properties().Select(BuildCollection));
        }

        private static ISolrClusterReplica BuildReplica(ISolrClusterCollection collection, JProperty json) {
            var baseUrl = (string) json.Value["base_url"];
            var leader = json.Value["leader"];
            var state = GetState(json);
            return new SolrClusterReplica {
                BaseUrl = baseUrl,
                IsLeader = leader != null && (bool) leader,
                Name = json.Name,
                NodeName = (string) json.Value["node_name"],
                State = state,
                IsActive = IsActive(state),
                Url = baseUrl + "/" + collection.Name
            };
        }

        private static ISolrClusterReplicas BuildReplicas(ISolrClusterCollection collection, JObject json) {
            return new SolrClusterReplicas(
                json.Properties().Select(property => BuildReplica(collection, property)));
        }

        private static ISolrClusterRouter BuildRouter(JObject json) {
            return new SolrClusterRouter {
                Name = (string) json["name"]
            };
        }

        private static ISolrClusterShard BuildShard(ISolrClusterCollection collection, JProperty json) {
            var state = GetState(json);
            return new SolrClusterShard {
                Name = json.Name,
                Range = SolrClusterShardRange.Parse((string) json.Value["range"]),
                State = state,
                IsActive = IsActive(state),
                Replicas = BuildReplicas(collection, json.Value["replicas"] as JObject)
            };
        }

        private static ISolrClusterShards BuildShards(ISolrClusterCollection collection, JObject json) {
            return new SolrClusterShards(
                json.Properties().Select(property => BuildShard(collection, property)));
        }

        private static string GetState(JProperty json) {
            return (string) json.Value["state"];
        }

        private static bool IsActive(string state) {
            return "active".Equals(state, StringComparison.OrdinalIgnoreCase);
        }

        public static ISolrClusterCollections ParseJsonToCollections(string json) {
            return BuildCollections(JObject.Parse(json));
        }
    }
}