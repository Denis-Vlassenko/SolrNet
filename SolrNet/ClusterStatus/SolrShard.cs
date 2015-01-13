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

using System.Collections.Generic;

namespace SolrNet.ClusterStatus {
    /// <summary>
    /// Represents shard of a Solr cluster.
    /// </summary>
    public class SolrShard {
        /// <summary>
        /// Initializes new instance of the <see cref="SolrShard"/> class.
        /// </summary>
        /// <param name="name">Name of this shard.</param>
        /// <param name="replicas">List of replicas in this shard.</param>
        /// <param name="state">Description of state of this shard.</param>
        public SolrShard(string name, IList<SolrReplica> replicas) {
            Name = name;
            SolrReplicas = replicas;
        }

        /// <summary>
        /// Gets name of this shard.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets list of replicas in this shard.
        /// </summary>
        public IList<SolrReplica> SolrReplicas { get; private set; }
    }
}
