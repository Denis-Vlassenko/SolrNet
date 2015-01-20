using System;
using System.Collections.Generic;
using System.Linq;

namespace SolrNet.Cloud {
    public abstract class SolrCloudOperationsBase<T> {
        protected SolrCloudOperationsBase(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, string collectionName = null)
        {
            this.collectionName = collectionName;
            this.cloudStateProvider = cloudStateProvider;
            this.operationsProvider = operationsProvider;
            random = new Random();
        }

        private readonly string collectionName;

        private readonly ISolrCloudStateProvider cloudStateProvider;

        private readonly ISolrOperationsProvider operationsProvider;

        private readonly Random random;

        protected TResult PerformBasicOperation<TResult>(Func<ISolrBasicOperations<T>, TResult> operation, bool leader = false)
        {
            var replicas = SelectReplicas(leader);
            var operations = operationsProvider.GetBasicOperations<T>(
                replicas[random.Next(replicas.Count)].Url);
            if (operations == null)
                throw new ApplicationException("Operations provider returned null.");
            return operation(operations);
        }

        protected TResult PerformOperation<TResult>(Func<ISolrOperations<T>, TResult> operation, bool leader = false) {
            var replicas = SelectReplicas(leader);
            var operations = operationsProvider.GetOperations<T>(
                replicas[random.Next(replicas.Count)].Url);
            if (operations == null)
                throw new ApplicationException("Operations provider returned null.");
            return operation(operations);
        }

        private IList<SolrCloudReplica> SelectReplicas(bool leaders) {
            var state = cloudStateProvider.GetCloudState();
            var collection = collectionName == null
                ? state.Collections.Values.First()
                : state.Collections[collectionName];
            var replicas = collection.Shards.Values
                .Where(shard => shard.IsActive)
                .SelectMany(shard => shard.Replicas.Values)
                .Where(replica => replica.IsActive && (!leaders || replica.IsLeader))
                .ToList();
            if (replicas.Count == 0)
                throw new ApplicationException("No appropriate node was selected to perform the operation.");
            return replicas;
        }
    }
}