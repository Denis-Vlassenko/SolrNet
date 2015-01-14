using System.IO;
using NUnit.Framework;

namespace SolrNet.Cloud.Tests {
    public class SolrClusterCoresTests {
        private string EmptyJson {
            get { return File.ReadAllText(@"resources\empty.json"); }
        }

        private string NotEmptyJson {
            get { return File.ReadAllText(@"resources\not-empty.json"); }
        }

        [Test]
        public void EmptyJsonDoesNotThrow() {
            Assert.DoesNotThrow(() => SolrClusterCores.Create(null, EmptyJson));
        }

        [Test]
        public void EmptyJsonProducesEmptyResult() {
            var cores = SolrClusterCores.Create(null, EmptyJson);
            Assert.False(cores.Count > 0);
        }

        [Test]
        public void NotEmptyJsonDoesNotThrow() {
            Assert.DoesNotThrow(() => SolrClusterCores.Create(null, NotEmptyJson));
        }

        [Test]
        public void NotEmptyJsonProducesNotEmptyResult() {
            var cores = SolrClusterCores.Create(null, NotEmptyJson);
            Assert.True(cores.Count > 0);
            Assert.True(cores.Default.Shards.Count > 0);
            Assert.True(cores.Default.Shards.Default.Replicas.Count > 0);
            Assert.True(cores.Default.Shards.Default.Replicas.Leader != null);
        }
    }
}