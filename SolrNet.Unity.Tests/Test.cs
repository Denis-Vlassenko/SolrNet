using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using SolrNet.Attributes;
using SolrNet.Cloud;

using CloudStartup = SolrNet.Cloud.Startup;

namespace SolrNet.Unity.Tests
{
    public class Test
    {
        [Test]
        public void TestSimple() {
            Startup.Init<TestEntity>("http://localhost:8983/solr/myconf");

            TestRoutine<ISolrOperations<TestEntity>>();
        }

        [Test]
        public void TestSorlCloud()
        {
            CloudStartup.Init<TestEntity>("10.26.11.30:9983");

            TestRoutine<ISolrCloudOperations<TestEntity>>();
        }
    
        public void TestRoutine<T>()
            where T : ISolrOperations<TestEntity>
        {
            var num = 1000;

            var solr = ServiceLocator.Current.GetInstance<T>();

            solr.Delete(SolrQuery.All);
            solr.Commit();

            // send one by one to test sharding distribution and sending to leaders only
            foreach (var ent in GenerateTestData(num))
                solr.Add(ent);

            solr.Commit();

            var results = solr.Query(SolrQuery.All);
            Assert.AreEqual(num, results.Count, "results count");
        }


        IEnumerable<TestEntity> GenerateTestData(int num)
        {
            for (int i = 0; i < num; i++)
            {
                yield return new TestEntity()
                {
                    Id = i.ToString(),
                    Name = "test" + i
                };
            }
        }
    }

    public class TestEntity {
        [SolrField("id")]
        public string Id { get; set; }
        [SolrField("name")]
        public string Name { get; set; }
    }
}
