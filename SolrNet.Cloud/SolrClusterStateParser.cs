using System;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud
{
    public static class SolrClusterStateParser
    {
        private static ISolrClusterCollection BuildCollection(ISolrCluster cluster, JProperty json) {
            var collection = new SolrClusterCollection {
                Cluster = cluster,
                Name = json.Name
            };
            collection.Router = BuildRouter(collection, json.Value["router"] as JObject);
            collection.Shards = BuildShards(collection, json.Value["shards"] as JObject);
            return collection;
        }

        private static ISolrClusterCollections BuildCollections(ISolrCluster cluster, JObject json) {
            var collections = new SolrClusterCollections {
                Cluster = cluster
            };
            foreach (var property in json.Properties())
            {
                var core = BuildCollection(cluster, property);
                if (collections.Count == 0)
                    collections.First = core;
                collections.Add(core.Name, core);
            }
            return collections;
        }

        private static ISolrClusterReplica BuildReplica(ISolrClusterShard shard, JProperty json) {
            var baseUrl = (string) json.Value["base_url"];
            var leader = json.Value["leader"];
            var state = GetState(json);
            return new SolrClusterReplica {
                BaseUrl = baseUrl,
                IsLeader = leader != null && (bool) leader,
                Name = json.Name,
                NodeName = (string) json.Value["node_name"],
                Shard = shard,
                State = state,
                IsActive = IsActive(state),
                Url = baseUrl + @"\" + shard.Collection.Name
            };
        }

        private static ISolrClusterReplicas BuildReplicas(ISolrClusterShard shard, JObject json) {
            var replicas = new SolrClusterReplicas();
            foreach (var property in json.Properties()) {
                var replica = BuildReplica(shard, property);
                if (replicas.Count == 0)
                    replicas.First = replica;
                if (replica.IsLeader)
                    replicas.Leader = replica;
                replicas.Add(replica);
            }
            return replicas;
        }

        private static ISolrClusterRouter BuildRouter(ISolrClusterCollection collection, JObject json) {
            return new SolrClusterRouter {
                Collection = collection,
                Name = (string)json["name"]
            };
        }

        private static ISolrClusterShard BuildShard(ISolrClusterCollection collection, JProperty json) {
            var state = GetState(json);
            var shard = new SolrClusterShard {
                Collection = collection,
                Name = json.Name,
                Range = SolrClusterShardRange.Parse((string) json.Value["range"]),
                
                State = state,
                IsActive = IsActive(state)
            };
            shard.Replicas = BuildReplicas(shard, json.Value["replicas"] as JObject);
            return shard;
        }

        private static ISolrClusterShards BuildShards(ISolrClusterCollection collection, JObject json) {
            var shards = new SolrClusterShards {
                Collection = collection
            };
            foreach (var property in json.Properties())
            {
                var shard = BuildShard(collection, property);
                if (shards.Count == 0)
                    shards.First = shard;
                shards.Add(shard.Name, shard);
            }
            return shards;
        }

        private static string GetState(JProperty json) {
            return (string) json.Value["state"];
        }

        private static bool IsActive(string state) {
            return "active".Equals(state, StringComparison.OrdinalIgnoreCase);
        }

        public static ISolrClusterCollections ParseJson(ISolrCluster cluster, string json)
        {
            return BuildCollections(cluster, JObject.Parse(json));
        }
    }
}
