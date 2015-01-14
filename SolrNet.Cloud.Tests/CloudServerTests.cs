using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SolrNet.Cloud.Tests
{
    class CloudServerTests
    {
        [Test]
        public void AddRemoveTest()
        {
            //var zkHost = "localhost:9983";
            //var collectionName = "myconf";
            //var num = 1000;

            //var solr = new SolrCloudServer2<TestEntity>(zkHost, collectionName);

            //solr.Delete(SolrQuery.All);
            //solr.Commit();

            //// send one by one to test sharding distribution and sending to leaders only
            //foreach (var ent in GenerateTestData(num))
            //    solr.Add(ent);

            //solr.Commit();

            //var results = solr.Query(SolrQuery.All);
            //Assert.AreEqual(num, results.Count, "results count");

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
}
