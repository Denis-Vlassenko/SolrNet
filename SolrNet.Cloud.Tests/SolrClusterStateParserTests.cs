using System.IO;
using NUnit.Framework;

namespace SolrNet.Cloud.Tests {
    public class SolrClusterStateParserTests {
        private string EmptyJson {
            get { return File.ReadAllText(@"resources\empty.json"); }
        }

        private string NotEmptyJson {
            get { return File.ReadAllText(@"resources\not-empty.json"); }
        }

        [Test]
        public void EmptyJsonDoesNotThrow() {
            Assert.DoesNotThrow(() => SolrClusterStateParser.ParseJson(EmptyJson));
        }

        [Test]
        public void EmptyJsonProducesEmptyResult() {
            var cores = SolrClusterStateParser.ParseJson(EmptyJson);
            Assert.False(cores.Count > 0);
        }

        [Test]
        public void NotEmptyJsonDoesNotThrow() {
            Assert.DoesNotThrow(() => SolrClusterStateParser.ParseJson(NotEmptyJson));
        }

        [Test]
        public void NotEmptyJsonProducesNotEmptyResult() {
            var collections = SolrClusterStateParser.ParseJson(NotEmptyJson);
            Assert.True(collections.Count > 0);
            Assert.True(collections[0].Shards.Count > 0);
            Assert.True(collections[0].Shards[0].Replicas.Count > 0);
            Assert.True(collections[0].Shards[0].Replicas.Leader != null);
        }
    }
}