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

namespace SolrNet.ClusterStatus
{
    /// <summary>
    /// Represents replica of a Solr cluster.
    /// </summary>
    public class SolrReplica {
        /// <summary>
        /// Initializes new instance of the <see cref="SolrReplica"/> class.
        /// </summary>
        /// <param name="baseUrl">Base url of this replica.</param>
        /// <param name="core">Name of the core corresponding this replica.</param>
        /// <param name="isActive">True if this replica is active.</param>
        /// <param name="isLeader">True if this replica is leader.</param>
        /// <param name="name">Name of this replica.</param>
        /// <param name="nodeName">Node name of this replica.</param>
        /// <param name="state">State description of this replica.</param>
        public SolrReplica(string baseUrl, string core, bool isActive, bool isLeader, string name, string nodeName, string state) {
            BaseUrl = baseUrl;
            Core = core;
            IsActive = isActive;
            IsLeader = isLeader;
            Name = name;
            NodeName = nodeName;
            State = state;
        }

        /// <summary>
        /// Gets base url of this replica.
        /// </summary>
        public string BaseUrl { get; private set; }

        /// <summary>
        /// Gets name of the core/collection this replica corresponds to.
        /// </summary>
        public string Core { get; private set; }

        /// <summary>
        /// Gets value indicating whether this replica is active or not.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets value indicating whether this replica is leader or not.
        /// </summary>
        public bool IsLeader { get; private set; }

        /// <summary>
        /// Gets name of this replica.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets name of the node of this replica.
        /// </summary>
        public string NodeName { get; private set; }

        /// <summary>
        /// Gets description of state of this replica.
        /// </summary>
        public string State { get; private set; }
    }
}