using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;

namespace SolrNet.Cloud.Tests
{
    class SolrClusterTests
    {
        [Test]
        public void AddRemoveTest()
        {
            const string CoreName = "myconf";
            const int MaxAttempts = 1;
            const int DocumentCount = 1000;
            const string ZkHostAddress = "10.26.11.30:9983";
            var balancer = new SolrClusterRandomBalancer();
            var provider = new SolrOperationsProvider();
            using (var cluster = new SolrCluster(balancer, MaxAttempts, provider, ZkHostAddress)) {
                Debug.Assert(cluster.Initialize());
                var operations = cluster.GetOperations<TestEntity>(CoreName);
                operations.Delete(SolrQuery.All);
                operations.Commit();
                //// send one by one to test sharding distribution and sending to leaders only
                foreach (var ent in GenerateTestData(DocumentCount))
                    operations.Add(ent);
                operations.Commit();
                var results = operations.Query(SolrQuery.All);
                Assert.AreEqual(DocumentCount, results.Count, "results count");
            }
        }
        
        IEnumerable<TestEntity> GenerateTestData(int num)
        {
            for (var i = 0; i < num; i++)
                yield return new TestEntity {
                    Id = i.ToString(),
                    Name = "test" + i
                };
        }
    }
}
