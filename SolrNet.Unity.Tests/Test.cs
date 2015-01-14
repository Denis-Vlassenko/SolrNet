using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using SolrNet.Attributes;
using SolrNet.Utils;

namespace SolrNet.Unity.Tests
{
    public class Test
    {
        [Test]
        public void TestSimple() {
            Startup.Init<TestEntity>("localhost:8983");

            TestRoutine();
        }

        [Test]
        public void TestSorlCloud()
        {
            // TODO DENIS - for cloud
            // probably new init method
            //Startup.InitCloud<TestEntity>("localhost:9983", "myconf");

            TestRoutine();
        }
    
        public void TestRoutine()
        {
            var num = 1000;

            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<TestEntity>>();

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
