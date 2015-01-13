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
    /// Represents status of a Solr cluster.
    /// </summary>
    public class SolrClusterStatus {
        /// <summary>
        /// Intializes new instance of the <see cref="SolrClusterStatus"/> class.
        /// </summary>
        /// <param name="collections">List of collections.</param>
        public SolrClusterStatus(IList<SolrCollection> collections) {
            SolrCollections = collections;
        }

        /// <summary>
        /// Gets list of collections in this cluster.
        /// </summary>
        public IList<SolrCollection> SolrCollections { get; private set; }
    }
}