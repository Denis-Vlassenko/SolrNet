using System.IO;
using System.Linq;
using NUnit.Framework;

namespace SolrNet.Cloud.Tests
{
    public class SolrClusterStateParserTests {
        private string EmptyJson {
            get { return File.ReadAllText(@"resources\empty.json"); }
        }

        private string NotEmptyJson
        {
            get { return File.ReadAllText(@"resources\not-empty.json"); }
        }

        [Test]
        public void EmptyJsonDoesNotThrow() {
            Assert.DoesNotThrow(() => SolrClusterStateParser.ParseJson(EmptyJson));
        }

        [Test]
        public void EmptyJsonProducesEmptyResult()
        {
            var result = SolrClusterStateParser.ParseJson(EmptyJson);
            Assert.False(result.Any());
        }

        [Test]
        public void NotEmptyJsonDoesNotThrow()
        {
            Assert.DoesNotThrow(() => SolrClusterStateParser.ParseJson(NotEmptyJson));
        }

        [Test]
        public void NotEmptyJsonProducesNotEmptyResult()
        {
            var result = SolrClusterStateParser.ParseJson(NotEmptyJson);
            Assert.True(result.Any());
            Assert.True(result.First().Shards.Any());
            Assert.True(result.First().Shards.First().Replicas.Any());
            Assert.True(result.First().Shards.First().Replicas.Active.Any());
            Assert.True(result.First().Shards.First().Replicas.Leaders.Any());
        }
    }
}
