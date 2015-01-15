using System.IO;
using System.Linq;
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
            Assert.DoesNotThrow(() => SolrCloudStateParser.ParseJsonToNodes(EmptyJson).ToArray());
        }

        [Test]
        public void EmptyJsonProducesEmptyResult() {
            Assert.False(SolrCloudStateParser.ParseJsonToNodes(EmptyJson).Any());
        }

        [Test]
        public void NotEmptyJsonDoesNotThrow() {
            Assert.DoesNotThrow(() => SolrCloudStateParser.ParseJsonToNodes(NotEmptyJson).ToArray());
        }

        [Test]
        public void NotEmptyJsonProducesNotEmptyResult() {
            var nodes = SolrCloudStateParser.ParseJsonToNodes(NotEmptyJson);
            Assert.True(nodes.Any());
        }
    }
}