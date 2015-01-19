using SolrNet.Attributes;

namespace SolrNet.Cloud.Tests {
    public class FakeEntity {
        [SolrField("id")]
        public string Id { get; set; }

        [SolrField("name")]
        public string Name { get; set; }
    }
}