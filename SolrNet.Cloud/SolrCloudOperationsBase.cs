using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SolrNet.Cloud {
    public abstract class SolrCloudOperationsBase<T> {
        protected SolrCloudOperationsBase(ISolrCloudState cloudState, ISolrOperationsProvider operationsProvider, string collectionName = null) {
            this.collectionName = collectionName;
            this.cloudState = cloudState;
            this.operationsProvider = operationsProvider;
            random = new Random();
        }

        private readonly string collectionName;

        private readonly ISolrCloudState cloudState;

        private readonly ISolrOperationsProvider operationsProvider;

        private readonly Random random;

        protected TResult PerformBasicOperation<TResult>(Func<ISolrBasicOperations<T>, TResult> operation, bool leader = false)
        {
            var nodes = SelectNodes(leader);
            var operations = operationsProvider.GetBasicOperations<T>(
                nodes[random.Next(nodes.Count)].Url);
            if (operations == null)
                throw new ApplicationException("Operations provider returned null.");
            return operation(operations);
        }

        protected TResult PerformOperation<TResult>(Func<ISolrOperations<T>, TResult> operation, bool leader = false) {
            var nodes = SelectNodes(leader);
            var operations = operationsProvider.GetOperations<T>(
                nodes[random.Next(nodes.Count)].Url);
            if (operations == null)
                throw new ApplicationException("Operations provider returned null.");
            return operation(operations);
        }

        private IList<ISolrCloudNode> SelectNodes(bool leader) {
            var nodes = cloudState.Nodes
                .Where(node => node.IsActive && (!leader || node.IsLeader))
                .Where(node => collectionName == null || collectionName.Equals(node.Collection, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (nodes.Count == 0)
                throw new ApplicationException("No appropriate node was selected to perform the operation.");
            return nodes;
        }
    }
}