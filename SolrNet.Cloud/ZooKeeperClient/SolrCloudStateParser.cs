using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud {
    public static class SolrCloudStateParser {
   
        public static SolrCloudState Parse(string json) {
            var collections = new Dictionary<string, SolrCloudCollection>();
            
            foreach (var collection in JObject.Parse(json).Properties())
            {
                var shards = new Dictionary<string, SolrCloudShard>();

                foreach (var shard in ((JObject)collection.Value["shards"]).Properties())
                {
                    var range = (string)shard.Value["range"];
                    int? rangeEnd = null;
                    int? rangeStart = null;
                    if (!string.IsNullOrEmpty(range))
                    {
                        var parts = range.Split('-');
                        rangeStart = int.Parse(parts[0], NumberStyles.HexNumber);
                        rangeEnd = int.Parse(parts[1], NumberStyles.HexNumber);
                    }

                    var replicas = new Dictionary<string, SolrCloudReplica>();
                    
                    foreach (var replica in ((JObject)shard.Value["replicas"]).Properties())
                        replicas.Add(replica.Name, new SolrCloudReplica(
                            (string)replica.Value["state"] == "active",
                            (string)replica.Value["leader"] == "true" , replica.Name,
                            (string)replica.Value["base_url"] + "/" + collection.Name));


                    shards.Add(shard.Name, new SolrCloudShard(
                        (string)shard.Value["state"] == "active",
                        shard.Name, rangeEnd, rangeStart, replicas));
                }

                var router = (string)collection.Value["router"]["name"];
                
                collections.Add(collection.Name, new SolrCloudCollection(collection.Name, new SolrCloudRouter(router), shards));
            }

            return new SolrCloudState(collections);
        }
    }
}