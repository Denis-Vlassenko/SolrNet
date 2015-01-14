using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SolrNet.Attributes;

namespace SolrNet.Cloud.Tests
{
    public class ZooKeeperClientTests
    {
        [Test]
        public void TestClusterState() {
            //var reader = new ZkStateReader();

            //var state = reader.GetClusterState("localhost:9983", "myconf");
        }

        [Test]
        public void JsonParseTest2() {

            //var state = ClusterState.FromJson(File.ReadAllText("resources\\clusterstate2.json"));

            //Assert.AreEqual(1, state.Collections.Count);
            //Assert.IsTrue(state.Collections.ContainsKey("myconf"), "myconf node do not exist");

            //var st = state.Collections["myconf"];
            //var myconf = new CollectionClusterState(st);

            //Assert.AreEqual(4, myconf.ActiveUrls.Count(), "active nodes");
            //Assert.AreEqual(2, myconf.Leaders.Count(), "leaders");
            //Assert.AreEqual(2, myconf.Collection.shards.Count,"shards");
        }

        [Test]
        public void JsonParseTest1()
        {
            //var state = ClusterState.FromJson(File.ReadAllText("resources\\clusterstate1.json"));

            //Assert.AreEqual(2, state.Collections.Count, "node count mismatch");

            //Assert.IsTrue(state.Collections.ContainsKey("offers"), "offers node do not exist");
            //var offersSt = state.Collections["offers"];
            //var offers = new CollectionClusterState(offersSt);

            //Assert.AreEqual(1, offers.ActiveUrls.Count(), "active nodes");
            //Assert.AreEqual(1, offers.Leaders.Count(), "leaders");
        }

        [Test]
        public void SimpleTest() {
            //var zkHost = "localhost:9983";
            //var collectionName = "myconf";

            //var state = new ZkStateReader().GetClusterState(zkHost, collectionName);

            //IEnumerable<string> activeUrls = state.ActiveUrls;
            //IEnumerable<string> leaderUrls = state.LeadersUrls;
                

            // var solr = SolrNet.GetServer<TestEntity>(activeUrls.First());

            // var results = solr.Query(SolrQuery.All);
        }


        


    }


    public class TestEntity {
        [SolrField("id")]
        public string Id { get; set; }
        [SolrField("name")]
        public string Name { get; set; }
    }
}
