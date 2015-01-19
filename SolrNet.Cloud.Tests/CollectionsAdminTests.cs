using System;
using NUnit.Framework;
using SolrNet.Cloud.CollectionsAdmin;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;


namespace SolrNet.Cloud.Tests
{
    [TestFixture]
    public class CollectionsAdminTests
    {
        public SolrConnection solrconnection;
        public SolrCollectionsAdmin collections;
        private const string name = "test";
        private const string confName = "myconf";


        [TestFixtureSetUp]
        public void Setup() {
            solrconnection = new SolrConnection("http://localhost:8983/solr");
            collections = new SolrCollectionsAdmin(solrconnection, new HeaderResponseParser<string>());
        }

        //[TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void ReloadColection() {
            var res = collections.ReloadCollection(confName);
            Assert.That(res.Status == 0);
        }

        [Test]
        public void CreateRemoveCollectionExcplicitRouter()
        {
            var res = collections.CreateCollection(name, configName: confName, numShards: 1);
            Assert.That(res.Status == 0);

            res = collections.DeleteCollection(name);
            Assert.That(res.Status == 0);
        }

        [Test]
        public void CreateRemoveCollectionImplicitRouter()
        {
            var res = collections.CreateCollection(name, configName: confName, routerName: "implicit", shards: "shard1, shard2", maxShardsPerNode:10);
            Assert.That(res.Status == 0);

            res = collections.DeleteCollection(name);
            Assert.That(res.Status == 0);
        }

        [Test]
        public void ZooTest() {
            // shoule be executed against one of livenodes of solrcloud

            //var liveNodes = ZooKeeper.GetLiveNodes();
        }

        [Test]
        public void AddRemoveShard()
        {
            RemoveCollectionIfExists(collections, name);
            try {
                var res = collections.CreateCollection(name, configName: confName, routerName: "implicit", shards: "shard1, shard2", maxShardsPerNode: 10);
                Assert.That(res.Status == 0);

                collections.CreateShard(name, "shard3");
                // Assert shard is created, check via cluster state
                collections.DeleteShard(name, "shard3");
            } catch (Exception e) {
                Assert.Fail(e.ToString());
            } finally {
                var res = collections.DeleteCollection(name);
                Assert.That(res.Status == 0);
            }
        }

        private void RemoveCollectionIfExists(ISolrCollectionsAdmin solr, string colName) {
            var list = solr.ListCollections();
            if (list.Contains(colName))
                solr.DeleteCollection(colName);
        }
    }
}
