using System;

namespace SolrNet.Cloud.Playground {
    public abstract class SolrClusterBalancerBase : ISolrClusterBalancer {
        protected SolrClusterBalancerBase(ISolrOperationsProvider provider) {
            if (provider == null)
                throw new ArgumentNullException("provider");
            this.provider = provider;
        }

        private readonly ISolrOperationsProvider provider;

        public TResult Balance<T, TResult>(Func<ISolrOperations<T>, TResult> operation, ISolrClusterReplicas replicas, bool leader) {
            var replica = leader ? replicas.Leader : SelectReplica(replicas);
            if (replica == null)
                throw new ApplicationException("No appropriate replica was selected to perform the operation.");
            var operations = provider.GetOperations<T>(replica);
            if (operations == null)
                throw new ApplicationException("Operation provider returned null.");
            try {
                return operation(operations);
            } catch {
                replica.Deactivate(); // todo: deactivate node only when status is not 401, 500
                throw;
            }
        }

        protected abstract ISolrClusterReplica SelectReplica(ISolrClusterReplicas replicas);
    }
}