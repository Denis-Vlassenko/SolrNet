using System.IO;
using System.Linq;
using NUnit.Framework;

namespace SolrNet.Cloud.Tests
{
    public class SolrClusterCoresTests {
        private string EmptyJson {
            get { return File.ReadAllText(@"resources\empty.json"); }
        }

        private string NotEmptyJson
        {
            get { return File.ReadAllText(@"resources\not-empty.json"); }
        }

        [Test]
        public void EmptyJsonDoesNotThrow() {
            Assert.DoesNotThrow(() => SolrClusterCores.ParseJson(EmptyJson));
        }

        [Test]
        public void EmptyJsonProducesEmptyResult()
        {
            var result = SolrClusterCores.ParseJson(EmptyJson);
            Assert.False(result.Any());
        }

        [Test]
        public void NotEmptyJsonDoesNotThrow()
        {
            Assert.DoesNotThrow(() => SolrClusterCores.ParseJson(NotEmptyJson));
        }

        [Test]
        public void NotEmptyJsonProducesNotEmptyResult()
        {
            var result = SolrClusterCores.ParseJson(NotEmptyJson);
            Assert.True(result.Any());
            Assert.True(result.First().Shards.Any());
            Assert.True(result.First().Shards.First().Replicas.Any());
            Assert.True(result.First().Shards.First().Replicas.Leader != null);
        }
    }
}
