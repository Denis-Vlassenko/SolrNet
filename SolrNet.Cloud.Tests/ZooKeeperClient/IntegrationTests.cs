using System;
using System.Collections.Generic;
using NUnit.Framework;
using SolrNet.Cloud.ZooKeeperClient;

namespace SolrNet.Cloud.Tests
{
    class SolrCloudStateTests {
        [Test]
        public void AddRemoveTest() {
            const int DocumentCount = 1000;
            const string ZooKeeperConnection = "127.0.0.1:9983";

            var state = new SolrCloudStateProvider(ZooKeeperConnection);
            state.Init();
            var operations = new SolrCloudOperations<FakeEntity>(state, new OperationsProvider());

            operations.Delete(SolrQuery.All);
            operations.Commit();
            
            //// send one by one to test sharding distribution and sending to leaders only
            foreach (var ent in GenerateTestData(DocumentCount))
                operations.Add(ent);
            operations.Commit();

            var results = operations.Query(SolrQuery.All);
            Assert.AreEqual(DocumentCount, results.Count, "results count");
        }

        IEnumerable<FakeEntity> GenerateTestData(int num)
        {
            for (var i = 0; i < num; i++)
                yield return new FakeEntity {
                    Id = i.ToString(),
                    Name = "test" + i
                };
        }

        private class OperationsProvider : ISolrOperationsProvider {
            public ISolrBasicOperations<T> GetBasicOperations<T>(string url) {
                throw new NotImplementedException();
            }

            public ISolrOperations<T> GetOperations<T>(string url) {
                return SolrNet.GetServer<T>(url);
            }
        }
    }
}
