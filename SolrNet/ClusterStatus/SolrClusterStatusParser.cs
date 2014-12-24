using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SolrNet.ClusterStatus {
    /// <summary>
    /// Parses a Solr cluster status xml document into a strongly typed
    /// <see cref="SolrClusterStatus"/> object.
    /// </summary>
    public class SolrClusterStatusParser : ISolrClusterStatusParser {
        /// <summary>
        /// Parses the specified Solr schema xml.
        /// </summary>
        /// <param name="solrClusterStatusXml">The Solr cluster status xml to parse.</param>
        /// <returns>A strongly styped representation of the Solr cluster status xml.</returns>
        public SolrClusterStatus Parse(XDocument solrClusterStatusXml) {
            var result = new SolrClusterStatus {
                SolrCollections = new List<SolrCollection>()
            };
            foreach (var collectionElem in solrClusterStatusXml.XPathSelectElements("//lst[@name='collections']/lst")) {
                var collection = new SolrCollection();
                result.SolrCollections.Add(collection);
                collection.Name = GetElementName(collectionElem);
                foreach (var shardElem in collectionElem.XPathSelectElements("lst[@name='shards']/lst")) {
                    var shard = new SolrShard();
                    collection.SolrShards.Add(shard);
                    shard.Name = GetElementName(shardElem);
                    foreach (var replicaElem in shardElem.XPathSelectElements("lst[@name='replicas']/lst")) {
                        var replica = new SolrReplica {
                            Name = GetElementName(replicaElem)
                        };
                        shard.SolrReplicas.Add(replica);
                        foreach (var element in replicaElem.Elements("str")) {
                            switch (GetElementName(element).ToLowerInvariant()) {
                                case "base_url":
                                    replica.BaseUrl = element.Value;
                                    break;
                                case "core":
                                    replica.Core = element.Value;
                                    break;
                                case "leader":
                                    replica.IsLeader = bool.TrueString.Equals(element.Value, StringComparison.OrdinalIgnoreCase);
                                    break;
                                case "node_name":
                                    replica.BaseUrl = element.Value;
                                    break;
                                case "state":
                                    replica.State = element.Value;
                                    replica.IsActive = "active".Equals(element.Value, StringComparison.OrdinalIgnoreCase);
                                    break;
                            }
                        }
                    }
                }
            }
            return result;
        }

        private static string GetElementName(XElement element) {
            var attribute = element.Attribute("name");
            return attribute == null ? string.Empty : attribute.Value;
        }
    }
}
