using System;
using System.Collections.Generic;

namespace SolrNet.Cloud.Playground {
    public abstract class SolrClusterBalancerBase : ISolrClusterBalancer {
        protected SolrClusterBalancerBase(ISolrOperationsProvider provider) {
            this.provider = provider;
        }

        private readonly ISolrOperationsProvider provider;

        public TResult Balance<T, TResult>(Func<ISolrOperations<T>, TResult> operation, ISolrClusterReplicas replicas, bool write) {
            while (true) {
                var replica = SelectReplica(write ? replicas.Leaders : replicas.Active);
                if (replica == null)
                    throw new ApplicationException("No appropriate replica was selected to perform the operation.");
                var operations = provider.GetOperations<T>(replica);
                if (operations == null)
                    throw new ApplicationException("No appropriate replica was selected to perform the operation.");
                try {
                    return operation(operations);
                } catch (Exception exception) {
                    replica.Deactivate(); // todo: deactivate node when status is not 401, 500
                }
            }
        }

        protected abstract ISolrClusterReplica SelectReplica(IEnumerable<ISolrClusterReplica> replicas);
    }
}