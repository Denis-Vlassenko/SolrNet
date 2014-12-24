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

using System.Xml.Linq;

namespace SolrNet.ClusterStatus {
    /// <summary>
    /// Provides an interface to parsing a solr cluster status xml document into a <see cref="SolrClusterStatus"/> object.
    /// </summary>
    public interface ISolrClusterStatusParser
    {
        /// <summary>
        /// Parses the specified solr cluster status XML.
        /// </summary>
        /// <param name="solrClusterStatusXml">The solr cluster status XML.</param>
        /// <returns>a object model of the solr cluster status.</returns>
        SolrClusterStatus Parse(XDocument solrClusterStatusXml);
    }
}
