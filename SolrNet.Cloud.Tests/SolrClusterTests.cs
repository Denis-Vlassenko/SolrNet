using System.Collections.Generic;
using NUnit.Framework;

namespace SolrNet.Cloud.Tests
{
    class SolrClusterTests
    {
        [Test]
        public void AddRemoveTest()
        {
            var balancer = new SolrClusterRandomBalancer();
            var coreName = "myconf";
            var maxAttempts = 1;
            var num = 1000;
            var provider = new SolrOperationsProvider();
            var zkHost = "localhost:9983";
            using (var cluster = new SolrCluster(balancer, maxAttempts, provider, zkHost)) {
                var operations = cluster.GetOperations<TestEntity>(coreName);
                operations.Delete(SolrQuery.All);
                operations.Commit();
                //// send one by one to test sharding distribution and sending to leaders only
                foreach (var ent in GenerateTestData(num))
                    operations.Add(ent);
                operations.Commit();
                var results = operations.Query(SolrQuery.All);
                Assert.AreEqual(num, results.Count, "results count");
            }
        }
        
        IEnumerable<TestEntity> GenerateTestData(int num)
        {
            for (var i = 0; i < num; i++)
            {
                yield return new TestEntity
                {
                    Id = i.ToString(),
                    Name = "test" + i
                };
            }
        }
    }
}
