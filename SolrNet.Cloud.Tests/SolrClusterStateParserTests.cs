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
            Assert.DoesNotThrow(() => SolrClusterStateParser.ParseJson(null, EmptyJson));
        }

        [Test]
        public void EmptyJsonProducesEmptyResult() {
            var cores = SolrClusterStateParser.ParseJson(null, EmptyJson);
            Assert.False(cores.Count > 0);
        }

        [Test]
        public void NotEmptyJsonDoesNotThrow() {
            Assert.DoesNotThrow(() => SolrClusterStateParser.ParseJson(null, NotEmptyJson));
        }

        [Test]
        public void NotEmptyJsonProducesNotEmptyResult() {
            var cores = SolrClusterStateParser.ParseJson(null, NotEmptyJson);
            Assert.True(cores.Count > 0);
            Assert.True(cores.First.Shards.Count > 0);
            Assert.True(cores.First.Shards.First.Replicas.Count > 0);
            Assert.True(cores.First.Shards.First.Replicas.Leader != null);
        }
    }
}