using System.Collections.Generic;
using NUnit.Framework;

namespace SolrNet.Cloud.Tests
{
    class ComplexTests
    {
        [Test]
        public void AddRemoveTest() {
            const int DocumentCount = 1000;
            const string ZooKeeperConnection = "10.26.11.30:9983";

            Startup.Init<TestEntity>(ZooKeeperConnection);
            var operations = Startup.Container.GetInstance<ISolrCloudOperations<TestEntity>>();

            operations.Delete(SolrQuery.All);
            operations.Commit();
            
            //// send one by one to test sharding distribution and sending to leaders only
            foreach (var ent in GenerateTestData(DocumentCount))
                operations.Add(ent);
            operations.Commit();

            var results = operations.Query(SolrQuery.All);
            Assert.AreEqual(DocumentCount, results.Count, "results count");
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
