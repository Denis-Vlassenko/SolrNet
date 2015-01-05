using NUnit.Framework;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrFacetPivotQueryTest {
        private static readonly SolrFacetPivotQuerySerializer serializer = new SolrFacetPivotQuerySerializer();

        [Test]
        public void SinglePivotTest() {
            var q = new SolrFacetPivotQuery {
                Fields = new[] {new PivotFields("manu_exact", "inStock")},
                MinCount = 1
            };

            var r = serializer.Serialize(q);
            CollectionAssert.Contains(r, KV.Create("facet.pivot", "manu_exact,inStock"));
            CollectionAssert.Contains(r, KV.Create("facet.pivot.mincount", "1"));
        }

        [Test]
        public void SinglePivotTestWithoutMinCount() {
            var q = new SolrFacetPivotQuery {
                Fields = new[] { new PivotFields("manu_exact","inStock")}
            };

            var r = serializer.Serialize(q);
            CollectionAssert.Contains(r, KV.Create("facet.pivot", "manu_exact,inStock"));
            foreach (var kvPair in r) {
                CollectionAssert.DoesNotContain(kvPair.Key, "facet.pivot.mincount");
            }
        }

        [Test]
        public void MultiplePivotTest() {
            var q = new SolrFacetPivotQuery {
                Fields = new[] { new PivotFields("manu_exact","inStock"), new PivotFields("inStock", "cat"), },
                MinCount = 1
            };

            var r = serializer.Serialize(q);
            CollectionAssert.Contains(r, KV.Create("facet.pivot", "manu_exact,inStock"));
            CollectionAssert.Contains(r, KV.Create("facet.pivot", "inStock,cat"));
            CollectionAssert.Contains(r, KV.Create("facet.pivot.mincount", "1"));
        }
    }
}