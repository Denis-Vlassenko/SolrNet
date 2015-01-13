#region license

// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SolrNet.ClusterStatus {
    /// <summary>
    /// Parses a Solr cluster status xml document into a strongly typed
    /// <see cref="SolrClusterStatus"/> object.
    /// </summary>
    public static class SolrClusterStatusParser {
        /// <summary>
        /// Parses the specified Solr schema xml.
        /// </summary>
        /// <param name="solrClusterStatusXml">The Solr cluster status xml to parse.</param>
        /// <returns>A strongly styped representation of the Solr cluster status xml.</returns>
        public static SolrClusterStatus Parse(XDocument solrClusterStatusXml) {
            var collections = new List<SolrCollection>();
            foreach (var collectionElem in solrClusterStatusXml.XPathSelectElements("//lst[@name='collections']/lst")) {
                var shards = new List<SolrShard>();
                foreach (var shardElem in collectionElem.XPathSelectElements("lst[@name='shards']/lst")) {
                    var replicas = new List<SolrReplica>();
                    foreach (var replicaElem in shardElem.XPathSelectElements("lst[@name='replicas']/lst")) {
                        var baseUrl = default(string);
                        var core = default(string);
                        var isActive = false;
                        var isLeader = false;
                        var nodeName = default(string);
                        var state = default(string);
                        foreach (var element in replicaElem.Elements("str")) {
                            switch (GetElementName(element).ToLowerInvariant()) {
                                case "base_url":
                                    baseUrl = element.Value;
                                    break;
                                case "core":
                                    core = element.Value;
                                    break;
                                case "leader":
                                    isLeader = bool.TrueString.Equals(element.Value, StringComparison.OrdinalIgnoreCase);
                                    break;
                                case "node_name":
                                    nodeName = element.Value;
                                    break;
                                case "state":
                                    state = element.Value;
                                    isActive = "active".Equals(element.Value, StringComparison.OrdinalIgnoreCase);
                                    break;
                            }
                        }
                        replicas.Add(new SolrReplica(baseUrl, core, isActive, isLeader, GetElementName(replicaElem), nodeName, state));
                    }
                    shards.Add(new SolrShard(GetElementName(shardElem), replicas));
                }
                collections.Add(new SolrCollection(GetElementName(collectionElem), shards));
            }
            return new SolrClusterStatus(collections);
        }

        /// <summary>
        /// Returns value of 'name' attribute of <paramref name="element"/> or empty string.
        /// </summary>
        private static string GetElementName(XElement element) {
            var attribute = element.Attribute("name");
            return attribute == null ? string.Empty : attribute.Value;
        }
    }
}
